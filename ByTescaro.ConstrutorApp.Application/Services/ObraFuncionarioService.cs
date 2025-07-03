using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraFuncionarioService : IObraFuncionarioService
    {
        private readonly IObraFuncionarioRepository _repo;
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IObraRepository _obraRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IFuncaoRepository _funcaoRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public ObraFuncionarioService(IObraFuncionarioRepository repo, IMapper mapper, IFuncionarioRepository funcionarioRepository, IObraRepository obraRepository, IProjetoRepository projetoRepository, IClienteRepository clienteRepository, IFuncaoRepository funcaoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _funcionarioRepository = funcionarioRepository;
            _obraRepository = obraRepository;
            _projetoRepository = projetoRepository;
            _clienteRepository = clienteRepository;
            _funcaoRepository = funcaoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ObraFuncionarioDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraFuncionarioDto>>(list);
        }

        public async Task CriarAsync(ObraFuncionarioDto dto)
        {
            var entity = _mapper.Map<ObraFuncionario>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _repo.Add(entity);
        }

        public async Task AtualizarAsync(ObraFuncionarioDto dto)
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

        public async Task<List<FuncionarioDto>> ObterFuncionariosDisponiveisAsync(long obraId)
        {
            var todosFuncionarios = await _funcionarioRepository.GetAllAsync();
            var funcionariosAlocados = await _repo.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = funcionariosAlocados
                .Where(f => f.ObraId != obraId)
                .Select(f => f.FuncionarioId)
                .ToHashSet();

            var disponiveis = todosFuncionarios
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<FuncionarioDto>>(disponiveis);
        }


        public async Task<List<FuncionarioDto>> ObterFuncionariosTotalDisponiveisAsync()
        {
            var todosFuncionarios = await _funcionarioRepository
                .GetAllIncludingAsync(f => f.Funcao); 

            var disponiveis = todosFuncionarios.ToList();

            return _mapper.Map<List<FuncionarioDto>>(disponiveis);
        }


        public async Task<List<FuncionarioDto>> ObterFuncionariosTotalAlocadosAsync()
        {
            // Busca todos os funcionarios alocados em alguma obra
            var funcionariosAlocados = await _repo.GetAllAsync(); // ObraFuncionario
            var obras = await _obraRepository.GetAllAsync();
            var proejtos = await _projetoRepository.GetAllAsync();
            var clientes = await _clienteRepository.GetAllAsync();
            var funcionarios = await _funcionarioRepository.GetAllAsync();
            var funcoes = await _funcaoRepository.GetAllAsync();

            var resultado = funcionariosAlocados
                .Select(eqAlocado =>
                {
                    var funcionario = funcionarios.FirstOrDefault(e => e.Id == eqAlocado.FuncionarioId);
                    var obra = obras.FirstOrDefault(o => o.Id == eqAlocado.ObraId);
                    var projeto = proejtos.FirstOrDefault(p => p.Id == obra.ProjetoId);
                    var cliente = clientes.FirstOrDefault(c => c.Id == projeto.ClienteId);
                    var funcao = funcoes.FirstOrDefault(f => f.Id == funcionario.FuncaoId);

                    return new FuncionarioDto
                    {
                        Id = funcionario?.Id ?? 0,
                        Nome = funcionario?.Nome ?? string.Empty,
                        ObraNome = obra?.Nome ?? string.Empty,
                        ProjetoNome = projeto?.Nome ?? string.Empty,
                        ClienteNome = cliente.Nome ?? string.Empty,
                        FuncaoNome = funcao.Nome
                    };
                }).ToList();

            return resultado;


        }


    }


}
