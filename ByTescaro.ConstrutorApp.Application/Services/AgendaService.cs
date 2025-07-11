using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;
        private readonly IUsuarioRepository _usuarioRepository; // Para obter dados do usuário
        private readonly INotificationService _zApiNotificationService; // Já existe
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILembreteEventoRepository _lembreteEventoRepository;

        public AgendaService(IEventoRepository eventoRepository,
                             IParticipanteEventoRepository participanteEventoRepository,
                             IUsuarioRepository usuarioRepository,
                             INotificationService zApiNotificationService,
                             IUnitOfWork unitOfWork,
                             IMapper mapper,
                             ILembreteEventoRepository lembreteEventoRepository)
        {
            _eventoRepository = eventoRepository;
            _participanteEventoRepository = participanteEventoRepository;
            _usuarioRepository = usuarioRepository;
            _zApiNotificationService = zApiNotificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _lembreteEventoRepository = lembreteEventoRepository;
        }

        public async Task<EventoDto> CriarEventoAsync(CriarEventoRequest request, long criadorId)
        {
            var evento = _mapper.Map<Evento>(request);
            evento.CriadorId = criadorId;
            evento.DataHoraCadastro = DateTime.Now;

            try
            {
                // 1. Salvar o Evento principal para obter o ID
                _unitOfWork.EventoRepository.Add(evento);
                await _unitOfWork.CommitAsync();

                if (evento.Id == 0)
                {
                    throw new InvalidOperationException("Erro: O evento não foi salvo e o ID não foi gerado.");
                }

                // Lista para armazenar todos os IDs de participantes que serão notificados
                // (Criador + Convidados). Isso nos permitirá buscar os nomes para a mensagem.
                var allParticipantIds = new List<long>(request.IdsParticipantesConvidados);
                allParticipantIds.Add(criadorId); // Adiciona o criador à lista de participantes para a mensagem

                // Obter todos os usuários participantes de uma vez para evitar múltiplas chamadas ao DB no loop de notificação
                // Usando GetByIdReadOnlyAsync ou FindAsync no UsuarioRepository que suporta múltiplos IDs
                // Se o seu IUsuarioRepository não tem um método para buscar por uma lista de IDs, você pode criar um
                // ou buscar individualmente (mas será menos eficiente para muitos usuários).
                var allParticipantsUsers = new List<Usuario>(); // Use o tipo da sua entidade Usuario
                foreach (var pId in allParticipantIds.Distinct()) // Distinct para evitar buscar o mesmo usuário múltiplas vezes
                {
                    var user = await _usuarioRepository.GetByIdTrackingAsync(pId); // Assumindo que este método existe e é ReadOnly
                    if (user != null)
                    {
                        allParticipantsUsers.Add(user);
                    }
                }

                // Construir a string de participantes para a mensagem
                var participantesNomes = allParticipantsUsers
                    .OrderBy(u => u.Nome) // Opcional: ordenar por nome
                    .Select(u => u.Nome)
                    .ToList();

                string participantesString = participantesNomes.Any()
                    ? string.Join(", ", participantesNomes)
                    : "Nenhum participante adicionado (além do criador)."; // Ajuste conforme necessário

                // 2. Adicionar o criador como participante aceito por padrão
                var criadorParticipante = new ParticipanteEvento
                {
                    EventoId = evento.Id,
                    UsuarioId = criadorId,
                    StatusParticipacao = StatusParticipacao.Aceito,
                    DataResposta = DateTime.Now
                };
                _unitOfWork.ParticipanteEventoRepository.Add(criadorParticipante);

                // 3. Adicionar os participantes convidados e enviar notificações
                foreach (var participanteId in request.IdsParticipantesConvidados)
                {
                    if (participanteId == criadorId) continue; // Evita duplicidade do criador

                    var participante = new ParticipanteEvento
                    {
                        EventoId = evento.Id,
                        UsuarioId = participanteId,
                        StatusParticipacao = StatusParticipacao.Pendente,
                        DataResposta = DateTime.Now
                    };
                    _unitOfWork.ParticipanteEventoRepository.Add(participante);

                    // Enviar convite via Z-API
                    var usuarioConvidado = allParticipantsUsers.FirstOrDefault(u => u.Id == participanteId);
                    if (usuarioConvidado != null && !string.IsNullOrEmpty(usuarioConvidado.TelefoneWhatsApp))
                    {
                        // Constrói a mensagem detalhada
                        var mensagemBuilder = new StringBuilder();
                        mensagemBuilder.AppendLine($"Olá *{usuarioConvidado.Nome}*, você foi convidado para um novo evento!");
                        mensagemBuilder.AppendLine("Você está relacionado ao seguinte compromisso:");
                        mensagemBuilder.AppendLine();
                        mensagemBuilder.AppendLine($"*Título*: {request.Titulo}");
                        mensagemBuilder.AppendLine($"*Início*: {request.DataHoraInicio:dd/MM/yyyy HH:mm}");
                        mensagemBuilder.AppendLine($"*Fim*: {request.DataHoraFim:dd/MM/yyyy HH:mm}");
                        mensagemBuilder.AppendLine($"*Conteúdo*:");
                        mensagemBuilder.AppendLine($"{request.Descricao}");
                        mensagemBuilder.AppendLine($"*Participantes*: {participantesString}");
                        mensagemBuilder.AppendLine("\nResponda 'Aceitar' ou 'Recusar'.");

                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuarioConvidado.TelefoneWhatsApp, mensagemBuilder.ToString());
                    }
                }

                // 4. Salvar os participantes e quaisquer outras mudanças
                await _unitOfWork.CommitAsync();

                // Opcional: Se precisar do nome do criador no EventoDto retornado, pode buscar aqui ou ajustar o mapper
                // evento.NomeCriador = allParticipantsUsers.FirstOrDefault(u => u.Id == criadorId)?.Nome;

                return _mapper.Map<EventoDto>(evento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar evento e registrar no banco de dados: {ex.Message}");
                // _logger.LogError(ex, "Erro ao criar evento e registrar no banco de dados.");
                throw new ApplicationException("Não foi possível criar o evento devido a um erro no banco de dados. Detalhes: " + ex.Message, ex);
            }
        }

        public async Task ResponderConviteAsync(RespostaConviteRequest request, long usuarioId)
        {
            var participanteEvento = await _participanteEventoRepository.GetByEventoAndUsuarioAsync(request.EventoId, usuarioId);

            if (participanteEvento == null)
            {
                throw new Exception("Convite não encontrado.");
            }

            participanteEvento.StatusParticipacao = request.StatusParticipacao;
            participanteEvento.DataResposta = DateTime.Now;

            _participanteEventoRepository.Update(participanteEvento);
            await _unitOfWork.CommitAsync();

            // Notificar o organizador sobre a resposta
            var evento = await _eventoRepository.GetByIdAsync(request.EventoId);
            var organizador = await _usuarioRepository.GetByIdAsync(evento.CriadorId);
            var respondente = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (organizador != null && !string.IsNullOrEmpty(organizador.TelefoneWhatsApp))
            {
                var mensagemResposta = $"{respondente.Nome} {request.StatusParticipacao.ToString().ToLower()} seu convite para o evento '{evento.Titulo}'.";
                await _zApiNotificationService.SendWhatsAppMessageAsync(organizador.TelefoneWhatsApp, mensagemResposta);
            }
        }

        public async Task EnviarLembretesProximosEventosAsync()
        {
            var agora = DateTime.Now;
            // Busca lembretes que devem ser enviados e ainda não foram
            var lembretesParaEnviar = await _lembreteEventoRepository.GetLembretesPendentesParaEnvioAsync(agora);

            foreach (var lembrete in lembretesParaEnviar)
            {
                var evento = await _eventoRepository.GetByIdAsync(lembrete.EventoId);
                if (evento == null) continue;

                var participantes = await _participanteEventoRepository.GetParticipantesByEventoIdAsync(evento.Id);

                foreach (var participante in participantes.Where(p => p.StatusParticipacao == StatusParticipacao.Aceito))
                {
                    var usuario = await _usuarioRepository.GetByIdAsync(participante.UsuarioId);
                    if (usuario != null && !string.IsNullOrEmpty(usuario.TelefoneWhatsApp))
                    {
                        var mensagem = $"Lembrete: O evento '{evento.Titulo}' começa em {lembrete.TempoAntes} {lembrete.UnidadeTempo}!";
                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuario.TelefoneWhatsApp, mensagem);
                    }
                }
                lembrete.Enviado = true; // Marca como enviado
                _lembreteEventoRepository.Update(lembrete);
            }
            await _unitOfWork.CommitAsync();
        }

        // Assumindo que este é o seu método AtualizarEventoAsync
        public async Task<EventoDto> AtualizarEventoAsync(AtualizarEventoRequest request, long usuarioId)
        {
            try // Mantendo o try-catch para diagnóstico de erros gerais
            {
                var eventoExistente = await _unitOfWork.EventoRepository.GetByIdAsync(request.Id); // Agora, eventoExistente está rastreado

                if (eventoExistente == null)
                {
                    throw new ApplicationException("Evento não encontrado para atualização.");
                }

                if (eventoExistente.CriadorId != usuarioId)
                {
                    throw new UnauthorizedAccessException("Você não tem permissão para editar este evento.");
                }

                // Mapeia as propriedades atualizáveis do request para a entidade existente e RASTREADA
                _mapper.Map(request, eventoExistente); // EF Core irá detectar as alterações aqui
                _unitOfWork.EventoRepository.Update(eventoExistente);

                // Atualizar participantes e enviar notificações
                // Esta seção pode precisar de cuidados extras para garantir que participantes sejam adicionados/removidos corretamente
                // e que suas entidades também sejam rastreadas ou reanexadas se necessário.
                var participantesAtuais = (await _unitOfWork.ParticipanteEventoRepository.GetParticipantesByEventoIdAsync(eventoExistente.Id)).ToList();
                var idsParticipantesAtuais = participantesAtuais.Select(p => p.UsuarioId).ToList();
                var idsParticipantesNovos = request.IdsParticipantesConvidados ?? new List<long>();

                // Remover participantes que não estão mais na lista
                var participantesParaRemover = participantesAtuais
                    .Where(p => !idsParticipantesNovos.Contains(p.UsuarioId) && p.UsuarioId != usuarioId) // Não remove o criador
                    .ToList();
                foreach (var participante in participantesParaRemover)
                {
                    _unitOfWork.ParticipanteEventoRepository.Remove(participante); // Remove do DbSet
                                                                        // Notificações Z-API para removidos
                    var usuarioRemovido = await _unitOfWork.UsuarioRepository.GetByIdAsync(participante.UsuarioId); // Pode estar AsNoTracking()
                    if (usuarioRemovido != null && !string.IsNullOrEmpty(usuarioRemovido.TelefoneWhatsApp))
                    {
                        var mensagem = $"Olá {usuarioRemovido.Nome}, o evento '{eventoExistente.Titulo}' foi atualizado e você foi removido como participante.";
                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuarioRemovido.TelefoneWhatsApp, mensagem);
                    }
                }

                // Adicionar novos participantes
                var idsNovosParticipantesParaAdicionar = idsParticipantesNovos
                    .Except(idsParticipantesAtuais)
                    .Where(id => id != usuarioId) // Garante que o criador não seja adicionado novamente
                    .ToList();
                foreach (var participanteId in idsNovosParticipantesParaAdicionar)
                {
                    var novoParticipante = new ParticipanteEvento
                    {
                        EventoId = eventoExistente.Id, // Vincula ao evento rastreado
                        UsuarioId = participanteId,
                        StatusParticipacao = StatusParticipacao.Pendente,
                        DataResposta = DateTime.Now
                    };
                    _unitOfWork.ParticipanteEventoRepository.Add(novoParticipante); // Adiciona ao DbSet
                                                                         // Notificações Z-API para novos
                    var usuarioAdicionado = await _usuarioRepository.GetByIdAsync(participanteId); // Pode estar AsNoTracking()
                    if (usuarioAdicionado != null && !string.IsNullOrEmpty(usuarioAdicionado.TelefoneWhatsApp))
                    {
                        var mensagem = $"Olá {usuarioAdicionado.Nome}, você foi convidado para o evento '{eventoExistente.Titulo}' que foi atualizado e ocorrerá de {eventoExistente.DataHoraInicio:dd/MM/yyyy HH:mm} a {eventoExistente.DataHoraFim:dd/MM/yyyy HH:mm}. Responda 'Aceitar' ou 'Recusar'.";
                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuarioAdicionado.TelefoneWhatsApp, mensagem);
                    }
                }

                // Notificar todos os participantes existentes (e o criador) sobre a atualização do evento
                // NOTA: Os participantesAtuais aqui podem não refletir as últimas adições/remoções
                // Se precisar notificar os recém-adicionados, considere buscar a lista novamente ou ajustar a lógica.
                foreach (var participante in participantesAtuais.Where(p => p.UsuarioId != usuarioId))
                {
                    var usuarioNotificar = await _unitOfWork.UsuarioRepository.GetByIdAsync(participante.UsuarioId);
                    if (usuarioNotificar != null && !string.IsNullOrEmpty(usuarioNotificar.TelefoneWhatsApp))
                    {
                        var mensagem = $"O evento '{eventoExistente.Titulo}' foi atualizado e ocorrerá de {eventoExistente.DataHoraInicio:dd/MM/yyyy HH:mm} a {eventoExistente.DataHoraFim:dd/MM/yyyy HH:mm}. Verifique os detalhes na aplicação.";
                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuarioNotificar.TelefoneWhatsApp, mensagem);
                    }
                }

                // REMOVIDO: _eventoRepository.Update(eventoExistente);
                // As mudanças em eventoExistente já serão rastreadas e salvas pelo CommitAsync
                await _unitOfWork.CommitAsync();

                return _mapper.Map<EventoDto>(eventoExistente);
            }
            catch (Exception ex)
            {
                // Log ou re-lança para diagnóstico
                Console.WriteLine($"Erro ao atualizar evento: {ex.Message}");
                throw; // Relança a exceção original para a camada superior lidar
            }
        }

        public async Task ExcluirEventoAsync(long eventoId, long usuarioId)
        {
            // 1. Get the main Evento entity. Crucially, use GetByIdTrackingAsync
            // to ensure EF Core tracks this instance for deletion.
            var evento = await _unitOfWork.EventoRepository.GetByIdTrackingAsync(eventoId);

            if (evento == null)
            {
                throw new Exception("Evento não encontrado.");
            }

            // Validate permission: only the creator can delete the event
            if (evento.CriadorId != usuarioId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para excluir este evento.");
            }

            // 2. Load related entities for removal.
            // It's vital that GetParticipantesByEventoIdAsync and GetLembretesByEventoIdAsync
            // (which you haven't shared) do NOT try to eagerly load and track the 'Evento' parent again
            // (i.e., avoid .Include(p => p.Evento) without AsNoTracking() if 'Evento' is already tracked).
            // They should ideally return the child entities (ParticipanteEvento, LembreteEvento) directly,
            // either tracked or untracked. Since you're calling Remove/RemoveRange on them,
            // it's fine if they are untracked; EF will attach them as Deleted when Remove/RemoveRange is called.
            var participantes = await _unitOfWork.ParticipanteEventoRepository
                                                .GetParticipantesByEventoIdAsync(evento.Id);

            var lembretesDoEvento = await _unitOfWork.LembreteEventoRepository
                                                    .GetLembretesByEventoIdAsync(evento.Id);

            // 3. Notify participants before removing them
            foreach (var participante in participantes)
            {
                // Use GetByIdReadOnlyAsync for Usuario, as you're only reading it for notification.
                var usuario = await _unitOfWork.UsuarioRepository.GetByIdTrackingAsync(participante.UsuarioId);
                if (usuario != null && !string.IsNullOrEmpty(usuario.TelefoneWhatsApp))
                {
                    var mensagem = $"O evento '{evento.Titulo}' que ocorreria em {evento.DataHoraInicio:dd/MM/yyyy HH:mm} foi cancelado.";
                    await _zApiNotificationService.SendWhatsAppMessageAsync(usuario.TelefoneWhatsApp, mensagem);
                }
            }

            // 4. Remove related entities (Lembretes and Participantes)
            // Using RemoveRange for efficiency, if available.
            if (lembretesDoEvento != null && lembretesDoEvento.Any())
            {
                _unitOfWork.LembreteEventoRepository.RemoveRange(lembretesDoEvento);
            }

            if (participantes != null && participantes.Any())
            {
                _unitOfWork.ParticipanteEventoRepository.RemoveRange(participantes);
            }

            // 5. Delete the main Evento entity.
            // This 'evento' object is the one already tracked by GetByIdTrackingAsync.
            _unitOfWork.EventoRepository.Remove(evento);

            // 6. Commit all changes to the database
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<EventoDto>> ObterEventosDoUsuarioAsync(long usuarioId)
        {
            var eventos = await _unitOfWork.EventoRepository.GetEventosByUsuarioIdAsync(usuarioId); // Este método deve buscar eventos onde o usuário é criador OU participante

            // É importante carregar os dados dos participantes e criadores para o DTO
            var eventosDto = new List<EventoDto>();
            foreach (var evento in eventos)
            {
                var eventoDto = _mapper.Map<EventoDto>(evento);
                eventoDto.NomeCriador = (await _unitOfWork.UsuarioRepository.GetByIdAsync(evento.CriadorId))?.Nome;

                var participantes = await _unitOfWork.ParticipanteEventoRepository.GetParticipantesByEventoIdAsync(evento.Id);
                eventoDto.Participantes = new List<ParticipanteEventoDto>();
                foreach (var participante in participantes)
                {
                    var participanteDto = _mapper.Map<ParticipanteEventoDto>(participante);
                    participanteDto.NomeUsuario = (await _unitOfWork.UsuarioRepository.GetByIdAsync(participante.UsuarioId))?.Nome;
                    eventoDto.Participantes.Add(participanteDto);
                }
                eventosDto.Add(eventoDto);
            }
            return eventosDto;
        }

        public async Task<EventoDto> ObterEventoPorIdAsync(long eventoId, long usuarioId)
        {
            var evento = await _unitOfWork.EventoRepository.GetByIdAsync(eventoId);

            if (evento == null)
            {
                return null; // Ou lançar uma exceção NotFound
            }

            // Verificar se o usuário tem permissão para ver o evento
            // 1. É o criador?
            // 2. É um participante?
            // 3. O evento é público? (Se você tiver essa lógica de visibilidade)
            var ehCriador = evento.CriadorId == usuarioId;
            var ehParticipante = (await _unitOfWork.ParticipanteEventoRepository.GetByEventoAndUsuarioAsync(eventoId, usuarioId)) != null;
             var ehPublico = evento.Visibilidade == Visibilidade.Publico; // Se você implementar o Enum VisibilidadeEvento

            if (!ehCriador && !ehParticipante && !ehPublico)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar este evento.");
            }

            var eventoDto = _mapper.Map<EventoDto>(evento);
            eventoDto.NomeCriador = (await _unitOfWork.UsuarioRepository.GetByIdAsync(evento.CriadorId))?.Nome;

            var participantes = await _participanteEventoRepository.GetParticipantesByEventoIdAsync(evento.Id);
            eventoDto.Participantes = new List<ParticipanteEventoDto>();
            foreach (var participante in participantes)
            {
                var participanteDto = _mapper.Map<ParticipanteEventoDto>(participante);
                participanteDto.NomeUsuario = (await _unitOfWork.UsuarioRepository.GetByIdAsync(participante.UsuarioId))?.Nome;
                eventoDto.Participantes.Add(participanteDto);
            }

            return eventoDto;
        }
    }
}