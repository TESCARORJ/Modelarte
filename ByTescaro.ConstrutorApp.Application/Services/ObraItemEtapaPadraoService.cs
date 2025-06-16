using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoService : IObraItemEtapaPadraoService
    {
        private readonly IObraItemEtapaPadraoRepository _repo;
        private readonly IObraEtapaPadraoRepository _obraEtapaPadraoRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ObraItemEtapaPadraoService(
            IObraItemEtapaPadraoRepository repo, 
            IMapper mapper, 
            IObraEtapaPadraoRepository obraEtapaPadraoRepository, 
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _obraEtapaPadraoRepository = obraEtapaPadraoRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<ObraItemEtapaPadraoDto?> ObterPorIdAsync(long id)
{
    var entity = await _repo.GetByIdAsync(id);
    return _mapper.Map<ObraItemEtapaPadraoDto>(entity);
}


        public async Task<List<ObraItemEtapaPadraoDto>> ObterTodasAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(list);
        }

        public async Task<List<ObraItemEtapaPadraoDto>> ObterPorEtapaIdAsync(long etapaId)
        {
            var itens = await _repo.GetByEtapaPadraoIdAsync(etapaId);
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(itens);
        }

        public async Task AtualizarConclusaoAsync(long itemId, bool concluido)
        {
            var item = await _repo.GetByIdAsync(itemId);
            if (item == null) return;

            // Como este é um item padrão, normalmente ele não teria um campo "Concluido"
            // Este método deve ser ignorado ou removido da interface
            // Caso queira manter, você deve adicionar esse campo na entidade
        }

        public async Task CriarAsync(ObraItemEtapaPadraoDto dto)
        {
            var entity = _mapper.Map<ObraItemEtapaPadrao>(dto);

            entity.ObraEtapaPadrao = dto.ObraEtapaId > 0 ? await _obraEtapaPadraoRepository.GetByIdAsync(dto.ObraEtapaId) : null;
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            await _repo.AddAsync(entity);
        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.RemoveAsync(entity);
        }
    }
}
