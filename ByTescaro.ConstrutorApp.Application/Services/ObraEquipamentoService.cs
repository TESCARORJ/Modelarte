using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraEquipamentoService : IObraEquipamentoService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly ILogAuditoriaRepository _logRepo;



        public ObraEquipamentoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService, ILogAuditoriaRepository logRepo)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
            _logRepo = logRepo;
        }

        public async Task<List<ObraEquipamentoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraEquipamentoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraEquipamentoDto>>(list);
        }

        public async Task CriarAsync(ObraEquipamentoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraEquipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraEquipamentoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraEquipamentoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraEquipamentoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log.
            var obraEquipamentoAntigoParaAuditoria = await _unitOfWork.ObraEquipamentoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraEquipamentoAntigoParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Obra Equipamento com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraEquipamentoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas.
            var obraEquipamentoParaAtualizar = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(dto.Id);

            if (obraEquipamentoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraEquipamentoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Obra Equipamento com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraEquipamentoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraEquipamentoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraEquipamentoAntigoParaAuditoria':
            // obraEquipamentoParaAtualizar.UsuarioCadastroId = obraEquipamentoAntigoParaAuditoria.UsuarioCadastroId;
            // obraEquipamentoParaAtualizar.DataHoraCadastro = obraEquipamentoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.ObraEquipamentoRepository.Update(obraEquipamentoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraEquipamentoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraEquipamentoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraEquipamentoAntigoParaAuditoria, obraEquipamentoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraEquipamentoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task<List<EquipamentoDto>> ObterEquipamentosDisponiveisAsync(long obraId)
        {
            var todosEquipamentos = await _unitOfWork.EquipamentoRepository.GetAllAsync();
            var equipamentosAlocados = await _unitOfWork.ObraEquipamentoRepository.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = equipamentosAlocados
                .Where(f => f.ObraId != obraId)
                .Select(f => f.EquipamentoId)
                .ToHashSet();

            var disponiveis = todosEquipamentos
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<EquipamentoDto>>(disponiveis);
        }

        public async Task<List<EquipamentoDto>> ObterEquipamentosTotalDisponiveisAsync()
        {
            var todosEquipamentos = await _unitOfWork.EquipamentoRepository.GetAllAsync();
            var equipamentosAlocados = await _unitOfWork.ObraEquipamentoRepository.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = equipamentosAlocados
                .Where(f => f.ObraId > 0)
                .Select(f => f.EquipamentoId)
                .ToHashSet();

            var disponiveis = todosEquipamentos
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<EquipamentoDto>>(disponiveis);
        }


        public async Task<List<EquipamentoDto>> ObterEquipamentosTotalAlocadosAsync()
        {
            // Busca todos os registros de alocação de equipamentos
            // Incluindo as entidades Obra, Equipamento, Projeto e Cliente para obter os nomes
            var equipamentosAlocadosQuery = await _unitOfWork.ObraEquipamentoRepository.GetAllAsync(); // ObraEquipamento

            var equipamentosAlocados = equipamentosAlocadosQuery
                .OrderByDescending(oe => oe.DataInicioUso) // Ordena para pegar o registro mais recente primeiro
                .ToList();

            var obras = (await _unitOfWork.ObraRepository.GetAllAsync()).ToList();
            var projetos = (await _unitOfWork.ProjetoRepository.GetAllAsync()).ToList();
            var clientes = (await _unitOfWork.ClienteRepository.GetAllAsync()).ToList();
            var equipamentos = (await _unitOfWork.EquipamentoRepository.GetAllAsync()).ToList();

            // Criar um dicionário para fácil acesso aos equipamentos pelo ID
            var equipamentosDict = equipamentos.ToDictionary(e => e.Id);
            var obrasDict = obras.ToDictionary(o => o.Id);
            var projetosDict = projetos.ToDictionary(p => p.Id);
            var clientesDict = clientes.ToDictionary(c => c.Id);

            // Lista para armazenar os EquipamentoDto com informações de alocação
            var resultadoEquipamentosDto = new List<EquipamentoDto>();

            // Para cada equipamento base, encontra o registro de alocação MAIS RECENTE
            // para preencher os campos de "atual"
            foreach (var equipBase in equipamentos)
            {
                var equipDto = _mapper.Map<EquipamentoDto>(equipBase);

                // Encontrar o registro de alocação mais recente para este equipamento (se houver)
                var ultimaAlocacao = equipamentosAlocados
                    .Where(oe => oe.EquipamentoId == equipBase.Id)
                    .OrderByDescending(oe => oe.DataInicioUso)
                     .ThenByDescending(oe => oe.Id)
                    .FirstOrDefault();

                if (ultimaAlocacao != null)
                {
                    // Preencher informações da alocação atual/última
                    equipDto.ObraIdAtual = ultimaAlocacao.ObraId;
                    equipDto.DataInicioUsoAtual = ultimaAlocacao.DataInicioUso;
                    equipDto.DataFimUsoAtual = ultimaAlocacao.DataFimUso;

                    if (obrasDict.TryGetValue(ultimaAlocacao.ObraId, out var obra))
                    {
                        equipDto.ObraNomeAtual = obra.Nome;
                        if (projetosDict.TryGetValue(obra.ProjetoId, out var projeto))
                        {
                            equipDto.ProjetoNomeAtual = projeto.Nome;
                            if (clientesDict.TryGetValue((long)projeto.ClienteId, out var cliente))
                            {
                                equipDto.ClienteNomeAtual = cliente.Nome;
                            }
                        }
                    }
                }
                resultadoEquipamentosDto.Add(equipDto);
            }

            return resultadoEquipamentosDto;
        }

        public async Task MoverEquipamentoAsync(MovimentacaoEquipamentoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;
            var usuarioLogadoNome = usuarioLogado?.Nome ?? "Sistema/Desconhecido"; // Nome para o log

            // 1. Encontrar o registro de alocação do equipamento na obra de origem
            // Usar GetByIdAsync ou GetOneWithIncludesAsync para garantir o rastreamento,
            // ou GetByIdNoTrackingAsync e então Attach/Update explicitamente.
            // Para ter o objeto rastreado e poder modificá-lo, use GetOneWithIncludesAsync SEM NoTracking().
            var equipamentoNaOrigem = await _unitOfWork.ObraEquipamentoRepository
                .FindOneWithIncludesAsync(oe => oe.ObraId == dto.ObraOrigemId && oe.EquipamentoId == dto.EquipamentoId && !oe.DataFimUso.HasValue);

            if (equipamentoNaOrigem == null)
            {
                throw new KeyNotFoundException($"Equipamento {dto.EquipamentoId} não encontrado na obra de origem {dto.ObraOrigemId} ou já finalizado.");
            }

            // Para auditoria, crie uma cópia ANTES das modificações
            var equipamentoOrigemAntigoParaAuditoria = new ObraEquipamento
            {
                Id = equipamentoNaOrigem.Id,
                ObraId = equipamentoNaOrigem.ObraId,
                EquipamentoId = equipamentoNaOrigem.EquipamentoId,
                EquipamentoNome = equipamentoNaOrigem.EquipamentoNome, // Certifique-se de copiar o nome também
                DataInicioUso = equipamentoNaOrigem.DataInicioUso,
                DataFimUso = equipamentoNaOrigem.DataFimUso,
                UsuarioCadastroId = equipamentoNaOrigem.UsuarioCadastroId,
                DataHoraCadastro = equipamentoNaOrigem.DataHoraCadastro
            };

            // 2. Encerrar o uso do equipamento na obra de origem
            equipamentoNaOrigem.DataFimUso = dto.DataMovimentacao;
            // Se equipamentoNaOrigem foi obtido com rastreamento (FindOneWithIncludesAsync sem NoTracking),
            // não é necessário chamar .Update() explicitamente, o EF Core detectará a mudança.
            // Se tivesse sido com NoTracking, seria necessário: _unitOfWork.ObraEquipamentoRepository.Update(equipamentoNaOrigem);

            // Registrar a atualização (finalização) na auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria

            {

                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,

                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,

                Entidade = nameof(Equipamento),

                TipoLogAuditoria = TipoLogAuditoria.Atualizacao,

                Descricao = $"Equipamento {equipamentoOrigemAntigoParaAuditoria.EquipamentoNome}. Movimentação: Equipamento finalizado na obra de origem. Motivo: {dto.Motivo} por '{usuarioLogadoNome}' em {DateTime.Now}. ",

                DadosAtuais = JsonSerializer.Serialize(equipamentoOrigemAntigoParaAuditoria) // Serializa o DTO para o log

            });



            // 3. Criar um novo registro de alocação para a obra de destino
            var equipamentoDetails = await _unitOfWork.EquipamentoRepository.GetByIdAsync(dto.EquipamentoId);
            if (equipamentoDetails == null)
            {
                throw new KeyNotFoundException($"Equipamento com ID {dto.EquipamentoId} não encontrado no cadastro de equipamentos.");
            }

            // Verifica se o equipamento já está alocado ativamente na obra de destino
            // Isso evita criar múltiplos registros ativos para o mesmo equipamento na mesma obra
            var equipamentoJaAlocadoNoDestino = await _unitOfWork.ObraEquipamentoRepository
                .FindOneWithIncludesNoTrackingAsync(oe => oe.ObraId == dto.ObraDestinoId && oe.EquipamentoId == dto.EquipamentoId && !oe.DataFimUso.HasValue);

            if (equipamentoJaAlocadoNoDestino != null)
            {
                // Se já existe um registro ativo, talvez você queira:
                // a) Lançar um erro para o usuário
                // b) Atualizar a data de início do registro existente (menos comum para movimentação)
                // c) Ignorar (se a regra de negócio permitir múltiplas entradas com datas de início diferentes)
                throw new InvalidOperationException($"Equipamento '{equipamentoDetails.Nome}' (ID: {dto.EquipamentoId}) já está alocado e ativo na obra de destino (ID: {dto.ObraDestinoId}).");
            }


            var novoEquipamentoAlocado = new ObraEquipamento
            {
                ObraId = dto.ObraDestinoId,
                EquipamentoId = dto.EquipamentoId,
                EquipamentoNome = equipamentoDetails.Nome, // Preencher nome do equipamento
                DataInicioUso = dto.DataMovimentacao,
                UsuarioCadastroId = usuarioLogadoId,
                DataHoraCadastro = DateTime.Now // Data e hora do cadastro do NOVO registro
            };

            // Adicionar o novo registro de alocação
            _unitOfWork.ObraEquipamentoRepository.Add(novoEquipamentoAlocado);

            // Registrar a criação (alocação na nova obra) na auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria

            {

                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,

                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,

                Entidade = nameof(Equipamento),

                TipoLogAuditoria = TipoLogAuditoria.Criacao,

                Descricao = $"Equipamento {novoEquipamentoAlocado.EquipamentoNome}. Movimentação: Equipamento alocado na obra de destino. Motivo: {dto.Motivo} por '{usuarioLogadoNome}' em {DateTime.Now}. ",

                DadosAtuais = JsonSerializer.Serialize(novoEquipamentoAlocado) // Serializa o DTO para o log

            });

            // 4. Salvar todas as alterações no banco de dados
            await _unitOfWork.CommitAsync();
        }
    }

}
