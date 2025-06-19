using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoInsumoService : IObraItemEtapaPadraoInsumoService
    {
        private readonly IObraItemEtapaPadraoInsumoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public ObraItemEtapaPadraoInsumoService(IObraItemEtapaPadraoInsumoRepository repo, IMapper mapper, IHttpContextAccessor http)
        {
            _repo = repo;
            _mapper = mapper;
            _http = http;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraItemEtapaPadraoInsumoDto>> ObterPorItemPadraoIdAsync(long itemPadraoId)
        {
            var lista = await _repo.GetByItemPadraoIdAsync(itemPadraoId);
            return _mapper.Map<List<ObraItemEtapaPadraoInsumoDto>>(lista);
        }

        public async Task CriarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var entidade = _mapper.Map<ObraItemEtapaPadraoInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastro = Usuario;
            await _repo.AddAsync(entidade);
        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var entidade = await _repo.GetByIdAsync(dto.Id);
            if (entidade == null) return;

            _mapper.Map(dto, entidade);
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
