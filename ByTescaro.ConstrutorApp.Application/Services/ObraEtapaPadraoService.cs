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
        // Obtém o ID do usuário logado (assincronamente para evitar .Result)
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
        // Essa instância 'obraEtapaPadraoAntigaParaAuditoria' NÃO será modificada pelo AutoMapper,
        // preservando o estado original para o log de auditoria.
        var obraEtapaPadraoAntigaParaAuditoria = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdNoTrackingAsync(dto.Id);

        if (obraEtapaPadraoAntigaParaAuditoria == null)
        {
            // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
            throw new KeyNotFoundException($"Obra Etapa Padrão com ID {dto.Id} não encontrada para auditoria.");
        }

        // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
        // Essa instância 'obraEtapaPadraoParaAtualizar' é a que o EF Core está monitorando
        // e que terá suas propriedades alteradas e salvas no banco de dados.
        var obraEtapaPadraoParaAtualizar = await _unitOfWork.ObraEtapaPadraoRepository.GetByIdAsync(dto.Id);

        if (obraEtapaPadraoParaAtualizar == null)
        {
            // Isso deve ser raro se 'obraEtapaPadraoAntigaParaAuditoria' foi encontrado,
            // mas é uma boa verificação de segurança para o fluxo de atualização.
            throw new KeyNotFoundException($"Obra Etapa Padrão com ID {dto.Id} não encontrada para atualização.");
        }

        // 3. Mapeie as propriedades do DTO para a entidade 'obraEtapaPadraoParaAtualizar' (a rastreada).
        // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
        _mapper.Map(dto, obraEtapaPadraoParaAtualizar);

        // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
        // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
        // usando os valores de 'obraEtapaPadraoAntigaParaAuditoria'.
        // Exemplo:
        // obraEtapaPadraoParaAtualizar.UsuarioCadastroId = obraEtapaPadraoAntigaParaAuditoria.UsuarioCadastroId;
        // obraEtapaPadraoParaAtualizar.DataHoraCadastro = obraEtapaPadraoAntigaParaAuditoria.DataHoraCadastro;

        // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
        // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
        // _unitOfWork.ObraEtapaPadraoRepository.Update(obraEtapaPadraoParaAtualizar);

        // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
        // 'obraEtapaPadraoAntigaParaAuditoria' tem os dados ANTES da mudança.
        // 'obraEtapaPadraoParaAtualizar' tem os dados DEPOIS da mudança.
        await _auditoriaService.RegistrarAtualizacaoAsync(obraEtapaPadraoAntigaParaAuditoria, obraEtapaPadraoParaAtualizar, usuarioLogadoId);

        // 5. Salve TODAS as alterações no banco de dados em uma única transação.
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
