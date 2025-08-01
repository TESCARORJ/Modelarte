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
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ObraEquipamentoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;


            var obraEquipamentoAntigoParaAuditoria = await _unitOfWork.ObraEquipamentoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraEquipamentoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Equipamento com ID {dto.Id} não encontrado para auditoria.");
            }


            var obraEquipamentoParaAtualizar = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(dto.Id);

            if (obraEquipamentoParaAtualizar == null)
            {
   
                throw new KeyNotFoundException($"Obra Equipamento com ID {dto.Id} não encontrado para atualização.");
            }

            _mapper.Map(dto, obraEquipamentoParaAtualizar);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(obraEquipamentoAntigoParaAuditoria, obraEquipamentoParaAtualizar, usuarioLogadoId);
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraEquipamentoRepository.Remove(entity);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

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
            var usuarioLogadoNome = usuarioLogado?.Nome ?? "Sistema/Desconhecido";


            var equipamentoNaOrigem = await _unitOfWork.ObraEquipamentoRepository
                .FindOneWithIncludesAsync(oe => oe.ObraId == dto.ObraOrigemId && oe.EquipamentoId == dto.EquipamentoId && !oe.DataFimUso.HasValue);

            if (equipamentoNaOrigem == null)
            {
                throw new KeyNotFoundException($"Equipamento {dto.EquipamentoId} não encontrado na obra de origem {dto.ObraOrigemId} ou já finalizado.");
            }

            var equipamentoOrigemAntigoParaAuditoria = new ObraEquipamento
            {
                Id = equipamentoNaOrigem.Id,
                ObraId = equipamentoNaOrigem.ObraId,
                EquipamentoId = equipamentoNaOrigem.EquipamentoId,
                EquipamentoNome = equipamentoNaOrigem.EquipamentoNome, 
                DataInicioUso = equipamentoNaOrigem.DataInicioUso,
                DataFimUso = equipamentoNaOrigem.DataFimUso,
                UsuarioCadastroId = equipamentoNaOrigem.UsuarioCadastroId,
                DataHoraCadastro = equipamentoNaOrigem.DataHoraCadastro,
                
                
            };

            equipamentoNaOrigem.DataFimUso = dto.DataMovimentacao;
            equipamentoNaOrigem.Equipamento.Status = StatusEquipamento.Indisponivel;
   
            await _logRepo.RegistrarAsync(new LogAuditoria

            {

                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,

                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,

                Entidade = nameof(Equipamento),

                TipoLogAuditoria = TipoLogAuditoria.Atualizacao,

                Descricao = $"Equipamento {equipamentoOrigemAntigoParaAuditoria.EquipamentoNome}. Movimentação: Equipamento finalizado na obra de origem. Motivo: {dto.Motivo} por '{usuarioLogadoNome}' em {DateTime.Now}. ",

                DadosAtuais = JsonSerializer.Serialize(equipamentoOrigemAntigoParaAuditoria)

            });



            var equipamentoDetails = await _unitOfWork.EquipamentoRepository.GetByIdAsync(dto.EquipamentoId);
            if (equipamentoDetails == null)
            {
                throw new KeyNotFoundException($"Equipamento com ID {dto.EquipamentoId} não encontrado no cadastro de equipamentos.");
            }

            var equipamentoJaAlocadoNoDestino = await _unitOfWork.ObraEquipamentoRepository
                .FindOneWithIncludesNoTrackingAsync(oe => oe.ObraId == dto.ObraDestinoId && oe.EquipamentoId == dto.EquipamentoId && !oe.DataFimUso.HasValue);

            if (equipamentoJaAlocadoNoDestino != null)
            {
                
                throw new InvalidOperationException($"Equipamento '{equipamentoDetails.Nome}' (ID: {dto.EquipamentoId}) já está alocado e ativo na obra de destino (ID: {dto.ObraDestinoId}).");
            }


            var novoEquipamentoAlocado = new ObraEquipamento
            {
                ObraId = dto.ObraDestinoId,
                EquipamentoId = dto.EquipamentoId,
                EquipamentoNome = equipamentoDetails.Nome, 
                DataInicioUso = dto.DataMovimentacao,
                UsuarioCadastroId = usuarioLogadoId,
                DataHoraCadastro = DateTime.Now 
            };

            _unitOfWork.ObraEquipamentoRepository.Add(novoEquipamentoAlocado);


            await _logRepo.RegistrarAsync(new LogAuditoria
            {

                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,

                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,

                Entidade = nameof(Equipamento),

                TipoLogAuditoria = TipoLogAuditoria.Criacao,

                Descricao = $"Equipamento {novoEquipamentoAlocado.EquipamentoNome}. Movimentação: Equipamento alocado na obra de destino. Motivo: {dto.Motivo} por '{usuarioLogadoNome}' em {DateTime.Now}. ",

                DadosAtuais = JsonSerializer.Serialize(novoEquipamentoAlocado) // Serializa o DTO para o log

            });

            await _unitOfWork.CommitAsync();
        }
    }

}
