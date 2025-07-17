using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraEquipamentoService : IObraEquipamentoService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;


        public ObraEquipamentoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
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
            // Busca todos os equipamentos alocados em alguma obra
            var equipamentosAlocados = await _unitOfWork.ObraEquipamentoRepository.GetAllAsync(); // ObraEquipamento
            var obras = await _unitOfWork.ObraRepository.GetAllAsync();
            var proejtos = await _unitOfWork.ProjetoRepository.GetAllAsync();
            var clientes = await _unitOfWork.ClienteRepository.GetAllAsync();
            var equipamentos = await _unitOfWork.EquipamentoRepository.GetAllAsync();

            var resultado = equipamentosAlocados
                .Select(eqAlocado =>
                {
                    var equipamento = equipamentos.FirstOrDefault(e => e.Id == eqAlocado.EquipamentoId);
                    var obra = obras.FirstOrDefault(o => o.Id == eqAlocado.ObraId);
                    var projeto = proejtos.FirstOrDefault(p => p.Id == obra.ProjetoId);
                    var cliente = clientes.FirstOrDefault(c => c.Id == projeto.ClienteId);

                    return new EquipamentoDto
                    {
                        Id = equipamento?.Id ?? 0,
                        Nome = equipamento?.Nome ?? string.Empty,
                        Patrimonio = equipamento?.Patrimonio ?? string.Empty,
                        ObraNome = obra?.Nome ?? string.Empty,
                        ProjetoNome = projeto?.Nome ?? string.Empty,
                        ClienteNome = cliente.Nome ?? string.Empty,
                    };
                }).ToList();

            return resultado;


        }

    }

}
