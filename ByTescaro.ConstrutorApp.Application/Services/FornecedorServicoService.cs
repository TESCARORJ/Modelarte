using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorServicoService : IFornecedorServicoService
    {
        private readonly IFornecedorServicoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public FornecedorServicoService(IFornecedorServicoRepository repo, IMapper mapper, IHttpContextAccessor http)
        {
            _repo = repo;
            _mapper = mapper;
            _http = http;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<FornecedorServicoDto>> ObterTodosAsync()
        {
            var lista = await _repo.GetAllAsync();
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<FornecedorServicoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _repo.GetByIdAsync(id);
            return entidade == null ? null : _mapper.Map<FornecedorServicoDto>(entidade);
        }

        public async Task<List<FornecedorServicoDto>> ObterPorFornecedorAsync(long fornecedorId)
        {
            var lista = await _repo.ObterPorFornecedorIdAsync(fornecedorId);
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<List<FornecedorServicoDto>> ObterPorServicoAsync(long servicoId)
        {
            var lista = await _repo.ObterPorServicoIdAsync(servicoId);
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<long> CriarAsync(FornecedorServicoDto dto)
        {
            var entidade = _mapper.Map<FornecedorServico>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastro = Usuario;
            await _repo.AddAsync(entidade);
            return entidade.Id;
        }

        public async Task AtualizarAsync(FornecedorServicoDto dto)
        {
            var entidade = _mapper.Map<FornecedorServico>(dto);
            await _repo.UpdateAsync(entidade);
        }

        public async Task RemoverAsync(long id)
        {
            var entidade = await _repo.GetByIdAsync(id);
            if (entidade != null)
                await _repo.RemoveAsync(entidade);
        }
    }

}
