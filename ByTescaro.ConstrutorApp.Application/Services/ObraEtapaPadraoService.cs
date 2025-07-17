using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

public class ObraEtapaPadraoService : IObraEtapaPadraoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public ObraEtapaPadraoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _auditoriaService = auditoriaService;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<List<ObraEtapaPadraoDto>> ObterTodasAsync()
    {
        var list = await _unitOfWork.ObraEtapaPadraoRepository.GetAllAsync();
        return _mapper.Map<List<ObraEtapaPadraoDto>>(list);
    }

    public async Task<ObraEtapaPadraoDto?> ObterPorIdAsync(long id)
    {
        var entity = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdAsync(id);
        return _mapper.Map<ObraEtapaPadraoDto>(entity);
    }

    public async Task CriarAsync(ObraEtapaPadraoDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = _mapper.Map<ObraEtapaPadrao>(dto);

        _unitOfWork.ObraEtapaPadraoRepository.Add(entity);
        await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

    public async Task AtualizarAsync(ObraEtapaPadraoDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entityAntigo = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdAsync(dto.Id);
        if (entityAntigo == null) return;

        var entityNovo = _mapper.Map(dto, entityAntigo);
        _unitOfWork.ObraEtapaPadraoRepository.Update(entityNovo);
        await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

    public async Task RemoverAsync(long id)
    {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdAsync(id);
        if (entity != null) _unitOfWork.ObraEtapaPadraoRepository.Remove(entity);
        await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

    public async Task<List<ObraEtapaPadraoDto>> ObterPorObraIdAsync(long obraId)
    {
        var result = await _unitOfWork.ObraEtapaPadraoRepository.GetByObraIdAsync(obraId);
        return _mapper.Map<List<ObraEtapaPadraoDto>>(result);
    }

    public async Task<ObraEtapaPadraoDto?> ObterComItensAsync(long etapaId)
    {
        var etapa = await _unitOfWork.ObraEtapaPadraoRepository.GetWithItensAsync(etapaId);
        return _mapper.Map<ObraEtapaPadraoDto>(etapa);
    }

    //public async Task AtualizarStatusAsync(long etapaId, StatusEtapa novoStatus)
    //{
    //    var etapa = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdAsync(etapaId);
    //    if (etapa == null) return;

    //    etapa.Status = novoStatus;
    //    _unitOfWork.ObraEtapaPadraoRepository.Update(etapa);
    //}
}
