using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraFornecedorService : IObraFornecedorService
    {
        private readonly IObraFornecedorRepository _repo;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IObraRepository _obraRepository;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IFuncaoRepository _funcaoRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public ObraFornecedorService(IObraFornecedorRepository repo, IMapper mapper, IFornecedorRepository fornecedorRepository, IObraRepository obraRepository, IProjetoRepository projetoRepository, IClienteRepository clienteRepository, IFuncaoRepository funcaoRepository, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _fornecedorRepository = fornecedorRepository;
            _obraRepository = obraRepository;
            _projetoRepository = projetoRepository;
            _clienteRepository = clienteRepository;
            _funcaoRepository = funcaoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ObraFornecedorDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraFornecedorDto>>(list);
        }

        public async Task CriarAsync(ObraFornecedorDto dto)
        {
            var entity = _mapper.Map<ObraFornecedor>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _repo.Add(entity);
        }

        public async Task AtualizarAsync(ObraFornecedorDto dto)
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

        public async Task<List<FornecedorDto>> ObterFornecedoresDisponiveisAsync(long obraId)
        {
            var todosFornecedores = await _fornecedorRepository.GetAllAsync();
            var fornecedoresAlocados = await _repo.GetAllAsync(); // Todos alocados em qualquer obra

            var idsAlocadosEmOutrasObras = fornecedoresAlocados
                .Where(f => f.ObraId != obraId)
                .Select(f => f.FornecedorId)
                .ToHashSet();

            var disponiveis = todosFornecedores
                .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
                .ToList();

            return _mapper.Map<List<FornecedorDto>>(disponiveis);
        }

        //public async Task<List<FornecedorDto>> ObterFornecedoresTotalDisponiveisAsync()
        //{
        //    var todosFornecedores = await _fornecedorRepository.GetAllAsync();
        //    var fornecedoresAlocados = await _repo.GetAllAsync(); // Todos alocados em qualquer obra

        //    var idsAlocadosEmOutrasObras = fornecedoresAlocados
        //        .Where(f => f.ObraId > 0)
        //        .Select(f => f.FornecedorId)

        //        .ToHashSet();

        //    var disponiveis = todosFornecedores
        //        .Where(f => !idsAlocadosEmOutrasObras.Contains(f.Id))
        //        .ToList();

        //    return _mapper.Map<List<FornecedorDto>>(disponiveis);
        //}

        public async Task<List<FornecedorDto>> ObterFornecedoresTotalDisponiveisAsync()
        {
            var todosFornecedores = await _fornecedorRepository.GetAllAsync(); 


            return _mapper.Map<List<FornecedorDto>>(todosFornecedores);
        }


        public async Task<List<FornecedorDto>> ObterFornecedoresTotalAlocadosAsync()
        {
            // Busca todos os fornecedores alocados em alguma obra
            var fornecedoresAlocados = await _repo.GetAllAsync(); // ObraFornecedor
            var obras = await _obraRepository.GetAllAsync();
            var proejtos = await _projetoRepository.GetAllAsync();
            var clientes = await _clienteRepository.GetAllAsync();
            var fornecedores = await _fornecedorRepository.GetAllAsync();
            var funcoes = await _funcaoRepository.GetAllAsync();

            var resultado = fornecedoresAlocados
                .Select(eqAlocado =>
                {
                    var fornecedor = fornecedores.FirstOrDefault(e => e.Id == eqAlocado.FornecedorId);
                    var obra = obras.FirstOrDefault(o => o.Id == eqAlocado.ObraId);
                    var projeto = proejtos.FirstOrDefault(p => p.Id == obra.ProjetoId);
                    var cliente = clientes.FirstOrDefault(c => c.Id == projeto.ClienteId);

                    return new FornecedorDto
                    {
                        Id = fornecedor?.Id ?? 0,
                        Nome = fornecedor?.Nome ?? string.Empty,
                       
                    };
                }).ToList();

            return resultado;


        }


    }


}
