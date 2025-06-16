using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorInsumoService : IFornecedorInsumoService
    {
        private readonly IFornecedorInsumoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public FornecedorInsumoService(IFornecedorInsumoRepository repo, IMapper mapper, IHttpContextAccessor http)
        {
            _repo = repo;
            _mapper = mapper;
            _http = http;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<FornecedorInsumoDto>> ObterTodosAsync()
        {
            var lista = await _repo.GetAllAsync();
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<FornecedorInsumoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _repo.GetByIdAsync(id);
            return entidade == null ? null : _mapper.Map<FornecedorInsumoDto>(entidade);
        }

        public async Task<List<FornecedorInsumoDto>> ObterPorFornecedorAsync(long fornecedorId)
        {
            var lista = await _repo.ObterPorFornecedorIdAsync(fornecedorId);
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<List<FornecedorInsumoDto>> ObterPorInsumoAsync(long insumoId)
        {
            var lista = await _repo.ObterPorInsumoIdAsync(insumoId);
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<long> CriarAsync(FornecedorInsumoDto dto)
        {
            var entidade = _mapper.Map<FornecedorInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastro = Usuario;
            await _repo.AddAsync(entidade);
            return entidade.Id;
        }

        public async Task AtualizarAsync(FornecedorInsumoDto dto)
        {
            var entidade = _mapper.Map<FornecedorInsumo>(dto);
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
