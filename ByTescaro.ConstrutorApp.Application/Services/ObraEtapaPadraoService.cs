using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

public class ObraEtapaPadraoService : IObraEtapaPadraoService
{
    private readonly IObraEtapaPadraoRepository _repo;
    private readonly IMapper _mapper;

    public ObraEtapaPadraoService(IObraEtapaPadraoRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<ObraEtapaPadraoDto>> ObterTodasAsync()
    {
        var list = await _repo.GetAllAsync();
        return _mapper.Map<List<ObraEtapaPadraoDto>>(list);
    }

    public async Task<ObraEtapaPadraoDto?> ObterPorIdAsync(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return _mapper.Map<ObraEtapaPadraoDto>(entity);
    }

    public async Task CriarAsync(ObraEtapaPadraoDto dto)
    {
        var entity = _mapper.Map<ObraEtapaPadrao>(dto);
        await _repo.AddAsync(entity);
    }

    public async Task AtualizarAsync(ObraEtapaPadraoDto dto)
    {
        var entity = await _repo.GetByIdAsync(dto.Id);
        if (entity == null) return;

        _mapper.Map(dto, entity);
        await _repo.UpdateAsync(entity);
    }

    public async Task RemoverAsync(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity != null) await _repo.RemoveAsync(entity);
    }

    public async Task<List<ObraEtapaPadraoDto>> ObterPorObraIdAsync(long obraId)
    {
        var result = await _repo.GetByObraIdAsync(obraId);
        return _mapper.Map<List<ObraEtapaPadraoDto>>(result);
    }

    public async Task<ObraEtapaPadraoDto?> ObterComItensAsync(long etapaId)
    {
        var etapa = await _repo.GetWithItensAsync(etapaId);
        return _mapper.Map<ObraEtapaPadraoDto>(etapa);
    }

    public async Task AtualizarStatusAsync(long etapaId, StatusEtapa novoStatus)
    {
        var etapa = await _repo.GetByIdAsync(etapaId);
        if (etapa == null) return;

        etapa.Status = novoStatus;
        await _repo.UpdateAsync(etapa);
    }
}
