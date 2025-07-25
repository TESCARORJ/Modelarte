using AutoMapper;
using Azure.Core;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json; // Para logs ou serialização/desserialização

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly IParticipanteEventoRepository _participanteEventoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly INotificationService _zApiNotificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILembreteEventoRepository _lembreteEventoRepository;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly ILogger<AgendaService> _logger;


        public AgendaService(IEventoRepository eventoRepository,
                             IParticipanteEventoRepository participanteEventoRepository,
                             IUsuarioRepository usuarioRepository,
                             INotificationService zApiNotificationService,
                             IUnitOfWork unitOfWork,
                             IMapper mapper,
                             ILembreteEventoRepository lembreteEventoRepository,
                             ILogAuditoriaRepository logRepo,
                             IUsuarioLogadoService usuarioLogadoService,
                             ILogger<AgendaService> logger)
        {
            _eventoRepository = eventoRepository;
            _participanteEventoRepository = participanteEventoRepository;
            _usuarioRepository = usuarioRepository;
            _zApiNotificationService = zApiNotificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _lembreteEventoRepository = lembreteEventoRepository;
            _logRepo = logRepo;
            _usuarioLogadoService = usuarioLogadoService;
            _logger = logger;
        }

        public async Task<EventoDto> CriarCompromissoAsync(CriarEventoRequest request, long UsuarioCadastroId)
        {
            var evento = _mapper.Map<Evento>(request);
            var usuarioLogado = await _usuarioRepository.GetByIdAsync(UsuarioCadastroId);
            evento.UsuarioCadastroId = UsuarioCadastroId;
            evento.DataHoraCadastro = DateTime.Now;



            try
            {
                _unitOfWork.EventoRepository.Add(evento);
                await _unitOfWork.CommitAsync();

                if (evento.Id == 0)
                {
                    throw new InvalidOperationException("Erro: O evento não foi salvo e o ID não foi gerado.");
                }

                var allParticipantIds = new List<long>(request.IdsParticipantesConvidados);
                allParticipantIds.Add(UsuarioCadastroId);
                var allParticipantsUsers = new List<Usuario>();

                foreach (var pId in allParticipantIds.Distinct())
                {
                    var user = await _usuarioRepository.GetByIdTrackingAsync(pId);
                    if (user != null)
                    {
                        allParticipantsUsers.Add(user);
                    }
                }

                var participantesNomes = allParticipantsUsers
                    .OrderBy(u => u.Nome)
                    .Select(u => u.Nome)
                    .ToList();

                string participantesString = participantesNomes.Any()
                    ? string.Join(", ", participantesNomes)
                    : "Nenhum participante adicionado (além do criador).";

                var criadorParticipante = new ParticipanteEvento
                {
                    EventoId = evento.Id,
                    UsuarioId = UsuarioCadastroId,
                    StatusParticipacao = StatusParticipacao.Aceito,
                    DataResposta = DateTime.Now
                };
                _unitOfWork.ParticipanteEventoRepository.Add(criadorParticipante);

                foreach (var participanteId in request.IdsParticipantesConvidados)
                {
                    if (participanteId == UsuarioCadastroId) continue;

                    var participante = new ParticipanteEvento
                    {
                        EventoId = evento.Id,
                        UsuarioId = participanteId,
                        StatusParticipacao = StatusParticipacao.Pendente,
                        DataResposta = default(DateTime) // Nulo até que o usuário responda
                    };
                    _unitOfWork.ParticipanteEventoRepository.Add(participante);




                    var usuarioConvidado = allParticipantsUsers.FirstOrDefault(u => u.Id == participanteId);
                    if (usuarioConvidado != null && !string.IsNullOrEmpty(usuarioConvidado.TelefoneWhatsApp))
                    {
                        var mensagemBase = new StringBuilder();
                        mensagemBase.AppendLine($"Olá *{usuarioConvidado.Nome}*!");
                        mensagemBase.AppendLine("Você está relacionado ao seguinte compromisso:");
                        mensagemBase.AppendLine();
                        mensagemBase.AppendLine($"*Título*: {request.Titulo}");
                        mensagemBase.AppendLine($"*Início*: {request.DataHoraInicio:dd/MM/yyyy HH:mm}");
                        mensagemBase.AppendLine($"*Fim*: {request.DataHoraFim:dd/MM/yyyy HH:mm}");
                        mensagemBase.AppendLine($"*Conteúdo*:");
                        mensagemBase.AppendLine($"{request.Descricao}");
                        mensagemBase.AppendLine($"*Participantes*: {participantesString}");

                        // O customId é fundamental para identificar o convite específico
                        var customId = $"EVENTO_CONVITE_{evento.Id}_{participanteId}";

                        await _zApiNotificationService.SendWhatsAppMessageAsync(
                            usuarioConvidado.TelefoneWhatsApp,
                            mensagemBase.ToString()
                        );
                    }
                }

                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                    UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                    Entidade = nameof(Evento),
                    TipoLogAuditoria = TipoLogAuditoria.Criacao,
                    Descricao = $"Compromisso criado por '{usuarioLogado}' em {DateTime.Now}. Título: {evento.Titulo} --- Início: {evento.DataHoraInicio} - Fim: {evento.DataHoraFim} ---  Descrição: {evento.Descricao} --- Paticipantes: {participantesString} ",
                    DadosAtuais = JsonSerializer.Serialize(request) // Serializa o DTO para o log
                });

                await _unitOfWork.CommitAsync();

                return _mapper.Map<EventoDto>(evento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar evento e registrar no banco de dados: {ex.Message}");
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

            var evento = await _eventoRepository.GetByIdAsync(request.EventoId);
            var organizador = await _usuarioRepository.GetByIdAsync((long)evento.UsuarioCadastroId);
            var respondente = await _usuarioRepository.GetByIdAsync(usuarioId);

            if (organizador != null && !string.IsNullOrEmpty(organizador.TelefoneWhatsApp))
            {
                var mensagemResposta = $"{respondente.Nome} {request.StatusParticipacao.ToString().ToLower()} seu convite para o evento '{evento.Titulo}'.";
                await _zApiNotificationService.SendWhatsAppMessageAsync(organizador.TelefoneWhatsApp, mensagemResposta);
            }
        }

        // NOVO MÉTODO: Para processar o webhook do Z-API
        public async Task ProcessarRespostaWebhookZApiAsync(ZApiWebhookMessage receivedMessage)
        {
            // Log do payload completo recebido para depuração
            Console.WriteLine($"Webhook Z-API recebido: {JsonSerializer.Serialize(receivedMessage)}");

            // O webhook 'on-message-received' pode trazer vários tipos de mensagens.
            // Filtramos as respostas de botões ou mensagens que contenham nosso customId.

            if (receivedMessage != null && !string.IsNullOrEmpty(receivedMessage.CustomId))
            {
                // Parse o customId. Ex: "EVENTO_CONVITE_123_456"
                var parts = receivedMessage.CustomId.Split('_');

                if (parts.Length == 4 && parts[0] == "EVENTO" && parts[1] == "CONVITE")
                {
                    if (long.TryParse(parts[2], out long eventoId) &&
                        long.TryParse(parts[3], out long usuarioId))
                    {
                        StatusParticipacao status;
                        string receivedContentLower = receivedMessage.Content?.ToLower(); // Use ?. para evitar NullReferenceException

                        if (receivedContentLower == "aceitar")
                        {
                            status = StatusParticipacao.Aceito;
                        }
                        else if (receivedContentLower == "recusar")
                        {
                            status = StatusParticipacao.Recusado;
                        }
                        else
                        {
                            Console.WriteLine($"Resposta inesperada para o convite '{receivedMessage.Content}'. Ignorando atualização de status para EventoId: {eventoId}, UsuarioId: {usuarioId}.");
                            return; // Não processa se o conteúdo não for "Aceitar" ou "Recusar"
                        }

                        // Busca o participante evento no banco
                        var participanteEvento = await _participanteEventoRepository.GetByEventoAndUsuarioAsync(eventoId, usuarioId);

                        if (participanteEvento != null && participanteEvento.StatusParticipacao == StatusParticipacao.Pendente)
                        {
                            participanteEvento.StatusParticipacao = status;
                            participanteEvento.DataResposta = DateTime.Now;
                            _participanteEventoRepository.Update(participanteEvento);
                            await _unitOfWork.CommitAsync();

                            // Opcional: Notificar o criador do evento sobre a resposta
                            var evento = await _eventoRepository.GetByIdAsync(eventoId);
                            if (evento != null)
                            {
                                var criadorEvento = await _usuarioRepository.GetByIdAsync((long)evento.UsuarioCadastroId);
                                if (criadorEvento != null && !string.IsNullOrEmpty(criadorEvento.TelefoneWhatsApp))
                                {
                                    var respondente = await _usuarioRepository.GetByIdAsync(usuarioId); // Busca o nome do respondente
                                    var mensagemParaCriador = $"{respondente?.Nome ?? "Um participante"} {status.ToString().ToLower()} seu convite para o evento '{evento.Titulo}'.";
                                    await _zApiNotificationService.SendWhatsAppMessageAsync(criadorEvento.TelefoneWhatsApp, mensagemParaCriador);
                                }
                            }
                        }
                        else if (participanteEvento != null && participanteEvento.StatusParticipacao != StatusParticipacao.Pendente)
                        {
                            Console.WriteLine($"Convite para o evento {eventoId} do participante {usuarioId} já foi respondido. Status atual: {participanteEvento.StatusParticipacao}.");
                        }
                        else
                        {
                            Console.WriteLine($"ParticipanteEvento não encontrado para EventoId: {eventoId}, UsuarioId: {usuarioId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Erro ao fazer parse de EventoId ou UsuarioId do CustomId: {receivedMessage.CustomId}");
                    }
                }
                else
                {
                    Console.WriteLine($"CustomId inesperado ou malformado: {receivedMessage.CustomId}");
                }
            }
            else
            {
                Console.WriteLine($"Webhook recebido sem customId ou com tipo não processado: Type={receivedMessage?.Type}, From={receivedMessage?.From}, Content={receivedMessage?.Content}");
            }
        }

        // Seus outros métodos do AgendaService permanecem inalterados
        public async Task EnviarLembretesProximosEventosAsync()
        {
            var agora = DateTime.Now;
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
                lembrete.Enviado = true;
                _lembreteEventoRepository.Update(lembrete);
            }
            await _unitOfWork.CommitAsync();
        }


        public async Task<EventoDto> AtualizarEventoAsync(AtualizarEventoRequest request, long usuarioId)
        {
            try
            {
                // 1. Buscar o evento existente COM RASTREAMENTO.
                // Isso garante que o EF Core conheça esta instância e evite conflitos.
                var eventoExistente = await _unitOfWork.EventoRepository.GetByIdTrackingAsync(request.Id);
                var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();

                if (eventoExistente == null)
                {
                    _logger.LogWarning($"Evento com ID {request.Id} não encontrado para atualização.");
                    throw new ApplicationException("Evento não encontrado para atualização.");
                }

                if (eventoExistente.UsuarioCadastroId != usuarioId)
                {
                    _logger.LogWarning($"Usuário {usuarioId} tentou editar evento {request.Id} sem permissão. Criador: {eventoExistente.UsuarioCadastroId}");
                    throw new UnauthorizedAccessException("Você não tem permissão para editar este evento.");
                }

                // 2. Buscar os participantes atuais COM RASTREAMENTO antes de qualquer modificação.
                var participantesAtuais = (await _unitOfWork.ParticipanteEventoRepository
                    .FindAllWithIncludesAsync(p => p.EventoId == request.Id, p => p.Usuario))
                    .ToList();

                // 3. Mapear as propriedades do request para a entidade JÁ RASTREADA.
                // O EF Core detectará automaticamente as alterações.
                _mapper.Map(request, eventoExistente);

                //_unitOfWork.EventoRepository.Update(eventoExistente);
                // porque a entidade já está sendo rastreada e as modificações são detectadas.

                var idsParticipantesAtuais = participantesAtuais.Select(p => p.UsuarioId).ToList();
                var idsParticipantesNovos = request.IdsParticipantesConvidados ?? new List<long>();
                string participantesString = string.Empty;

                // Remover participantes que não estão mais na lista
                var participantesParaRemover = participantesAtuais
                    .Where(p => !idsParticipantesNovos.Contains(p.UsuarioId) && p.UsuarioId != usuarioId)
                    .ToList();

                if (participantesParaRemover.Any())
                {
                    foreach (var participante in participantesParaRemover)
                    {
                        // Notificação para usuário removido
                        if (participante.Usuario != null && !string.IsNullOrEmpty(participante.Usuario.TelefoneWhatsApp))
                        {
                            var mensagem = $"Olá {participante.Usuario.Nome}, o evento '{eventoExistente.Titulo}' foi atualizado e você foi removido como participante.";
                            await _zApiNotificationService.SendWhatsAppMessageAsync(participante.Usuario.TelefoneWhatsApp, mensagem);
                        }
                    }
                    _unitOfWork.ParticipanteEventoRepository.RemoveRange(participantesParaRemover);
                }


                // Adicionar novos participantes
                var idsNovosParticipantesParaAdicionar = idsParticipantesNovos
                    .Except(idsParticipantesAtuais)
                    .ToList();

                foreach (var participanteId in idsNovosParticipantesParaAdicionar)
                {
                    var novoParticipante = new ParticipanteEvento
                    {
                        EventoId = eventoExistente.Id,
                        UsuarioId = participanteId,
                        StatusParticipacao = StatusParticipacao.Pendente
                    };
                    _unitOfWork.ParticipanteEventoRepository.Add(novoParticipante);

                    var usuarioAdicionado = await _usuarioRepository.GetByIdAsync(participanteId);
                    if (usuarioAdicionado != null && !string.IsNullOrEmpty(usuarioAdicionado.TelefoneWhatsApp))
                    {
                        var mensagemBase = new StringBuilder();
                        mensagemBase.AppendLine($"Olá *{usuarioAdicionado.Nome}*, você foi convidado para o evento '{eventoExistente.Titulo}' que foi atualizado!");
                        mensagemBase.AppendLine($"Ocorrerá de {eventoExistente.DataHoraInicio:dd/MM/yyyy HH:mm} a {eventoExistente.DataHoraFim:dd/MM/yyyy HH:mm}.");
                        await _zApiNotificationService.SendWhatsAppMessageAsync(
                            usuarioAdicionado.TelefoneWhatsApp,
                            mensagemBase.ToString()
                        );
                    }
                }

                // Notificar participantes que permaneceram no evento sobre as mudanças
                var idsParticipantesQuePermaneceram = idsParticipantesAtuais
                    .Intersect(idsParticipantesNovos)
                    .Where(id => id != usuarioId)
                    .ToList();

                var todosUsuariosParaNotificar = new List<long>();
                todosUsuariosParaNotificar.AddRange(idsNovosParticipantesParaAdicionar);
                todosUsuariosParaNotificar.AddRange(idsParticipantesQuePermaneceram);

                // Regenerar string de participantes para log/notificações
                var allFinalParticipantIds = new List<long>(idsParticipantesNovos) { usuarioId };
                var allFinalParticipantsUsers = new List<Usuario>();
                foreach (var pId in allFinalParticipantIds.Distinct())
                {
                    var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(pId);
                    if (user != null)
                    {
                        allFinalParticipantsUsers.Add(user);
                    }
                }
                participantesString = allFinalParticipantsUsers.Any()
                                    ? string.Join(", ", allFinalParticipantsUsers.OrderBy(u => u.Nome).Select(u => u.Nome))
                                    : "Nenhum participante adicionado (além do criador).";

                // Notificar apenas os que permaneceram sobre a atualização (os novos já receberam convite)
                foreach (var participanteId in idsParticipantesQuePermaneceram)
                {
                    var usuarioNotificar = await _unitOfWork.UsuarioRepository.GetByIdAsync(participanteId);
                    if (usuarioNotificar != null && !string.IsNullOrEmpty(usuarioNotificar.TelefoneWhatsApp))
                    {
                        var mensagemBase = new StringBuilder();
                        mensagemBase.AppendLine($"Olá *{usuarioNotificar.Nome}*!");
                        mensagemBase.AppendLine("O compromisso a seguir foi atualizado:");
                        mensagemBase.AppendLine();
                        mensagemBase.AppendLine($"*Título*: {request.Titulo}");
                        mensagemBase.AppendLine($"*Início*: {request.DataHoraInicio:dd/MM/yyyy HH:mm}");
                        mensagemBase.AppendLine($"*Fim*: {request.DataHoraFim:dd/MM/yyyy HH:mm}");
                        mensagemBase.AppendLine($"*Conteúdo*:");
                        mensagemBase.AppendLine($"{request.Descricao}");
                        mensagemBase.AppendLine($"*Participantes*: {participantesString}");

                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuarioNotificar.TelefoneWhatsApp, mensagemBase.ToString());
                    }
                }

                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado?.Id ?? 0,
                    UsuarioNome = usuarioLogado?.Nome ?? "Sistema/Desconhecido",
                    Entidade = nameof(Evento),
                    TipoLogAuditoria = TipoLogAuditoria.Atualizacao,
                    Descricao = $"Compromisso atualizado por '{usuarioLogado?.Nome ?? "Sistema"}' em {DateTime.Now}. Título: {eventoExistente.Titulo} --- Início: {eventoExistente.DataHoraInicio} - Fim: {eventoExistente.DataHoraFim} --- Descrição: {eventoExistente.Descricao} --- Participantes: {participantesString} ",
                    DadosAtuais = JsonSerializer.Serialize(request)
                });

                 await _unitOfWork.CommitAsync();

                return _mapper.Map<EventoDto>(eventoExistente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar evento: {ex.Message}");
                throw;
            }
        }



        public async Task ExcluirEventoAsync(long eventoId, long usuarioId)
        {
            var evento = await _unitOfWork.EventoRepository.GetByIdTrackingAsync(eventoId);
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();


            if (evento == null)
            {
                throw new Exception("Evento não encontrado.");
            }

            if (evento.UsuarioCadastroId != usuarioId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para excluir este evento.");
            }

            var participantes = await _unitOfWork.ParticipanteEventoRepository.GetParticipantesByEventoIdAsync(evento.Id);

            if (participantes.Count() > 1)
            {

                var lembretesDoEvento = await _unitOfWork.LembreteEventoRepository.GetLembretesByEventoIdAsync(evento.Id);

                var participantesNomes = participantes
                     .OrderBy(u => u.Usuario.Nome)
                     .Select(u => u.Usuario.Nome)
                     .ToList();
                string participantesString = participantes.Any() ? string.Join(", ", participantesNomes) : "Nenhum participante adicionado (além do criador).";

                foreach (var participante in participantes)
                {
                    var usuario = await _unitOfWork.UsuarioRepository.GetByIdTrackingAsync(participante.UsuarioId);
                    if (usuario != null && !string.IsNullOrEmpty(usuario.TelefoneWhatsApp))
                    {
                        var mensagem = $"O evento '{evento.Titulo}' que ocorreria em {evento.DataHoraInicio:dd/MM/yyyy HH:mm} foi cancelado.";
                        await _zApiNotificationService.SendWhatsAppMessageAsync(usuario.TelefoneWhatsApp, mensagem);
                    }
                }

                if (lembretesDoEvento != null && lembretesDoEvento.Any())
                {
                    _unitOfWork.LembreteEventoRepository.RemoveRange(lembretesDoEvento);
                }

                if (participantes != null && participantes.Any())
                {
                    _unitOfWork.ParticipanteEventoRepository.RemoveRange(participantes);
                }

                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                    UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                    Entidade = nameof(Evento),
                    TipoLogAuditoria = TipoLogAuditoria.Criacao,
                    Descricao = $"Compromisso excluído por '{usuarioLogado}' em {DateTime.Now}. Título: {evento.Titulo} --- Início: {evento.DataHoraInicio} - Fim: {evento.DataHoraFim} ---  Descrição: {evento.Descricao} --- Paticipantes: {participantesString} ",
                    DadosAtuais = JsonSerializer.Serialize(evento) // Serializa o DTO para o log
                });

            }
            _unitOfWork.EventoRepository.Remove(evento);

            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<EventoDto>> ObterEventosDoUsuarioAsync(long usuarioId)
        {
            var eventos = await _unitOfWork.EventoRepository.GetEventosByUsuarioIdAsync(usuarioId);

            var eventosDto = new List<EventoDto>();
            foreach (var evento in eventos)
            {
                var eventoDto = _mapper.Map<EventoDto>(evento);
                eventoDto.NomeCriador = (await _unitOfWork.UsuarioRepository.GetByIdAsync((long)evento.UsuarioCadastroId))?.Nome;

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
                return null;
            }

            var ehCriador = evento.UsuarioCadastroId == usuarioId;
            var ehParticipante = (await _unitOfWork.ParticipanteEventoRepository.GetByEventoAndUsuarioAsync(eventoId, usuarioId)) != null;
            var ehPublico = evento.Visibilidade == Visibilidade.Publico;

            if (!ehCriador && !ehParticipante && !ehPublico)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar este evento.");
            }

            var eventoDto = _mapper.Map<EventoDto>(evento);
            eventoDto.NomeCriador = (await _unitOfWork.UsuarioRepository.GetByIdAsync((long)evento.UsuarioCadastroId))?.Nome;

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

        public async Task<IEnumerable<EventoDto>> GetEventosByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Busca os eventos do repositório, incluindo Participantes e seus Usuários
            var eventos = await _unitOfWork.EventoRepository.GetEventosWithParticipantesAndUsuariosByDateRangeAsync(startDate, endDate);

            // Mapeia as entidades para DTOs
            return _mapper.Map<IEnumerable<EventoDto>>(eventos);
        }
    }
}