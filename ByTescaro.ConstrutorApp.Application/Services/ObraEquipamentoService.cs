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


        public ObraEquipamentoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }


        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraEquipamentoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraEquipamentoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraEquipamentoDto>>(list);
        }

        public async Task CriarAsync(ObraEquipamentoDto dto)
        {
            var entity = _mapper.Map<ObraEquipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _unitOfWork.ObraEquipamentoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraEquipamentoDto dto)
        {
            var entity = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            _unitOfWork.ObraEquipamentoRepository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraEquipamentoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraEquipamentoRepository.Remove(entity);
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
