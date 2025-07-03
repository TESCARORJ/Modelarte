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
        private readonly IObraEquipamentoRepository _repo;
        private readonly IObraRepository _obraRepository;
        private readonly IEquipamentoRepository _equipamentoRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ObraEquipamentoService(IObraEquipamentoRepository repo, IMapper mapper, IEquipamentoRepository equipamentoRepository, IObraRepository obraRepository, IProjetoRepository projetoRepository, IClienteRepository clienteRepository, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _equipamentoRepository = equipamentoRepository;
            _obraRepository = obraRepository;
            _projetoRepository = projetoRepository;
            _clienteRepository = clienteRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraEquipamentoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraEquipamentoDto>>(list);
        }

        public async Task CriarAsync(ObraEquipamentoDto dto)
        {
            var entity = _mapper.Map<ObraEquipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _repo.Add(entity);
        }

        public async Task AtualizarAsync(ObraEquipamentoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            _repo.Update(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                _repo.Remove(entity);
        }

        public async Task<List<EquipamentoDto>> ObterEquipamentosDisponiveisAsync(long obraId)
        {
            var todosEquipamentos = await _equipamentoRepository.GetAllAsync();
            var equipamentosAlocados = await _repo.GetAllAsync(); // Todos alocados em qualquer obra

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
            var todosEquipamentos = await _equipamentoRepository.GetAllAsync();
            var equipamentosAlocados = await _repo.GetAllAsync(); // Todos alocados em qualquer obra

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
            var equipamentosAlocados = await _repo.GetAllAsync(); // ObraEquipamento
            var obras = await _obraRepository.GetAllAsync();
            var proejtos = await _projetoRepository.GetAllAsync();
            var clientes = await _clienteRepository.GetAllAsync();
            var equipamentos = await _equipamentoRepository.GetAllAsync();

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
