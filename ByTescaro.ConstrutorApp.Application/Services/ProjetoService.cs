using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class ProjetoService : IProjetoService
{
    private readonly IProjetoRepository _repo;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IObraRepository _obraRepo;
    private readonly IObraEtapaRepository _obraEtapaRepo;
    private readonly IObraItemEtapaRepository _obraItemEtapaRepo;
    private readonly IObraRetrabalhoRepository _obraRetrabalhoRepo;
    private readonly IObraPendenciaRepository _obraPendenciaRepo;
    private readonly IObraDocumentoRepository _obraDocumentoRepo;
    private readonly IObraImagemRepository _obraImagemRepo;
    private readonly IObraFuncionarioRepository _obraFuncionarioRepo;
    private readonly IObraEquipamentoRepository _obraEquipamentoRepo;
    private readonly IObraInsumoListaRepository _obraInsumoListaRepo;


    public ProjetoService(
        IProjetoRepository repo,
        IAuditoriaService auditoriaService,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IObraRepository obraRepo,
        IObraEtapaRepository obraEtapaRepo,
        IObraItemEtapaRepository obraItemEtapaRepo,
        IObraRetrabalhoRepository obraRetrabalhoRepo,
        IObraDocumentoRepository obraDocumentoRepo,
        IObraImagemRepository obraImagemRepo,
        IObraFuncionarioRepository obraFuncioanarioRepo,
        IObraEquipamentoRepository obraEquipamentoRepo,
        IObraPendenciaRepository obraPendenciaRepo,
        IObraInsumoListaRepository obraInsumoListaRepo)
    {
        _repo = repo;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _obraRepo = obraRepo;
        _obraEtapaRepo = obraEtapaRepo;
        _obraItemEtapaRepo = obraItemEtapaRepo;
        _obraRetrabalhoRepo = obraRetrabalhoRepo;
        _obraDocumentoRepo = obraDocumentoRepo;
        _obraImagemRepo = obraImagemRepo;
        _obraFuncionarioRepo = obraFuncioanarioRepo;
        _obraEquipamentoRepo = obraEquipamentoRepo;
        _obraPendenciaRepo = obraPendenciaRepo;
        _obraInsumoListaRepo = obraInsumoListaRepo;
    }

    private string UsuarioLogado =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

    public async Task<IEnumerable<ProjetoDto>> ObterTodosAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var dtos = _mapper.Map<List<ProjetoDto>>(projetos);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }

    public async Task<IEnumerable<ProjetoListDto>> ObterTodosListAsync()
    {
        var dtos = await _repo.GetQueryable()
            .OrderBy(p => p.Nome)
            .Select(projeto => new ProjetoListDto // A variável 'projeto' é do tipo Projeto (entidade)
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Status = projeto.Status,
                DataInicio = projeto.DataInicio != null ? DateOnly.FromDateTime(projeto.DataInicio) : null,
                DataFim = projeto.DataFim.HasValue ? DateOnly.FromDateTime(projeto.DataFim.Value) : null,

                // Lógica de cálculo de progresso corrigida e otimizada para SQL
                ProgressoProjeto = projeto.Obras.Any()
                    ? (int)projeto.Obras.Average(obra => // Para cada obra, calcula seu progresso individual

                        // Verifica se a obra tem algum item para evitar divisão por zero
                        obra.Etapas.SelectMany(etapa => etapa.Itens).Any()
                            ?
                            // Calcula a porcentagem de itens concluídos para esta obra
                            ((double)obra.Etapas.SelectMany(etapa => etapa.Itens).Count(item => item.Concluido) /
                                      obra.Etapas.SelectMany(etapa => etapa.Itens).Count()) * 100
                            : 0 // Se a obra não tem itens, seu progresso é 0
                    )
                    : 0 // Se o projeto não tem obras, seu progresso é 0
            })
            .ToListAsync();

        return dtos;
    }
    // Em ProjetoService.cs
    public async Task<ProjetoDto?> ObterPorIdAsync(long id)
    {
        var projeto = await _repo.GetByIdAsync(id);
        if (projeto == null) return null;      
        return _mapper.Map<ProjetoDto>(projeto);
    }
    public async Task<ProjetoDto> CriarAsync(ProjetoDto dto)
    {
        var projeto = _mapper.Map<Projeto>(dto);
        projeto.UsuarioCadastro = UsuarioLogado;
        projeto.DataHoraCadastro = DateTime.Now;

        await _repo.AddAsync(projeto);

        foreach (var obraDto in dto.Obras)
        {
            var obra = _mapper.Map<Obra>(obraDto);
            obra.ProjetoId = projeto.Id;
            obra.UsuarioCadastro = UsuarioLogado;
            obra.DataHoraCadastro = DateTime.Now;
            obra.Etapas = new List<ObraEtapa>();

            foreach (var etapaDto in obraDto.Etapas)
            {
                var etapa = _mapper.Map<ObraEtapa>(etapaDto);
                etapa.ObraId = obra.Id;
                etapa.Itens = etapaDto.Itens.Select(itemDto => new ObraItemEtapa
                {
                    Nome = itemDto.Nome,
                    Ordem = itemDto.Ordem,
                    Concluido = itemDto.Concluido,
                    IsDataPrazo = itemDto.IsDataPrazo,
                    DiasPrazo = itemDto.DiasPrazo,
                    PrazoAtivo = itemDto.PrazoAtivo,
                    DataConclusao = itemDto.DataConclusao,
                    UsuarioCadastro = UsuarioLogado,
                    DataHoraCadastro = DateTime.Now
                }).ToList();
                obra.Etapas.Add(etapa);
            }

            #region RETRABALHO

            obra.Retrabalhos = obraDto.Retrabalhos.Select(retrabalho => new ObraRetrabalho
            {
                Titulo = retrabalho.Titulo,
                Descricao = retrabalho.Descricao,
                Status = retrabalho.Status,
                ResponsavelId = retrabalho.ResponsavelId,
                DataInicio = retrabalho.DataInicio,
                DataConclusao = retrabalho.DataConclusao,
                UsuarioCadastro = UsuarioLogado,
                DataHoraCadastro = DateTime.Now
            }).ToList();

            #endregion

            #region Pendencia

            obra.Pendencias = obraDto.Pendencias.Select(pendencia => new ObraPendencia
            {
                Titulo = pendencia.Titulo,
                Descricao = pendencia.Descricao,
                Status = pendencia.Status,
                ResponsavelId = pendencia.ResponsavelId,
                DataInicio = pendencia.DataInicio,
                DataConclusao = pendencia.DataConclusao,
                UsuarioCadastro = UsuarioLogado,
                DataHoraCadastro = DateTime.Now
            }).ToList();

            #endregion

            #region Documentos

            obra.Documentos = obraDto.Documentos?.Select(doc => new ObraDocumento
            {
                NomeOriginal = doc.NomeOriginal,
                CaminhoRelativo = doc.CaminhoRelativo,
                Extensao = doc.Extensao,
                TamanhoEmKb = doc.TamanhoEmKb,
                UsuarioCadastro = UsuarioLogado,
                DataHoraCadastro = DateTime.Now
            }).ToList() ?? new();

            #endregion

            #region Imagems 

            obra.Imagens = obraDto.Imagens?.Select(img => new ObraImagem
            {
                NomeOriginal = img.NomeOriginal,
                CaminhoRelativo = img.CaminhoRelativo,
                TamanhoEmKb = img.TamanhoEmKb,
                UsuarioCadastro = UsuarioLogado,
                DataHoraCadastro = DateTime.Now
            }).ToList() ?? new();

            #endregion 

           

            await _obraRepo.AddAsync(obra);


        }

        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<ProjetoDto>(projeto), UsuarioLogado);
        return _mapper.Map<ProjetoDto>(projeto); 

    }
    private void AtualizarCamposEtapa(ObraEtapa destino, ObraEtapaDto origem)
    {
        destino.Nome = origem.Nome;
        destino.Ordem = origem.Ordem;
        destino.Status = origem.Status;
        destino.DataInicio = origem.DataInicio;
        destino.DataConclusao = origem.DataConclusao;
        // UsuarioCadastro e DataHoraCadastro são preservados
    }
    private void AtualizarCamposItem(ObraItemEtapa destino, ObraItemEtapaDto origem)
    {
        destino.Nome = origem.Nome;
        destino.Ordem = origem.Ordem;
        destino.Concluido = origem.Concluido;
        destino.IsDataPrazo = origem.IsDataPrazo;
        destino.DiasPrazo = origem.DiasPrazo;
        destino.PrazoAtivo = origem.PrazoAtivo;
        destino.DataConclusao = origem.DataConclusao;
        // UsuarioCadastro e DataHoraCadastro são preservados
    }
    private void AtualizarCamposRetrabalho(ObraRetrabalho destino, ObraRetrabalhoDto origem)
    {
        destino.Titulo = origem.Titulo;
        destino.Descricao = origem.Descricao;
        destino.Status = origem.Status;
        destino.ResponsavelId = origem.ResponsavelId;
        destino.DataInicio = origem.DataInicio;
        destino.DataConclusao = origem.DataConclusao;
        // DataHoraCadastro e UsuarioCadastro são preservados
    }
    private void AtualizarCamposPendencia(ObraPendencia destino, ObraPendenciaDto origem)
    {
        destino.Titulo = origem.Titulo;
        destino.Descricao = origem.Descricao;
        destino.Status = origem.Status;
        destino.ResponsavelId = origem.ResponsavelId;
        destino.DataInicio = origem.DataInicio;
        destino.DataConclusao = origem.DataConclusao;
        // DataHoraCadastro e UsuarioCadastro são preservados
    }
    public async Task AtualizarAsync(ProjetoDto dto)
    {
        var projeto = await _repo.GetByIdAsync(dto.Id);
        if (projeto == null) return;

        _mapper.Map(dto, projeto);
        _repo.AnexarEntidade(projeto);

        var obrasAtuais = await _obraRepo.GetByProjetoIdAsync(projeto.Id);
        var idsRemovidos = obrasAtuais
            .Where(obraAntiga => !dto.Obras.Any(o => o.Id == obraAntiga.Id))
            .Select(o => o.Id)
            .ToList();

        foreach (var idRemovido in idsRemovidos)
        {
            var obra = obrasAtuais.First(o => o.Id == idRemovido);
            await _obraRepo.RemoveAsync(obra);
            var dtoObra = dto.Obras.FirstOrDefault(o => o.Id == idRemovido);
            if (dtoObra != null)
                dto.Obras.Remove(dtoObra);
        }

        foreach (var obraDto in dto.Obras)
        {
            if (obraDto.Id == 0)
            {
                var novaObra = _mapper.Map<Obra>(obraDto);
                novaObra.ProjetoId = projeto.Id;
                novaObra.UsuarioCadastro = UsuarioLogado;
                novaObra.DataHoraCadastro = DateTime.Now;
                await _obraRepo.AddAsync(novaObra);
            }
            else
            {
                var obraExistente = obrasAtuais.FirstOrDefault(o => o.Id == obraDto.Id);
                if (obraExistente != null)
                {
                    _mapper.Map(obraDto, obraExistente);
                    await _obraRepo.UpdateAsync(obraExistente);

                    #region Etapa
                    var etapasAtuais = await _obraEtapaRepo.GetByObraIdAsync(obraExistente.Id);
                    var etapasRemovidas = etapasAtuais
                        .Where(e => !obraDto.Etapas.Any(dtoE => dtoE.Id == e.Id))
                        .ToList();

                    if (etapasRemovidas.Count > 0)
                    {
                        foreach (var etapaRemovida in etapasRemovidas)
                            await _obraEtapaRepo.RemoveAsync(etapaRemovida);
                    }

                    foreach (var etapaDto in obraDto.Etapas)
                    {
                        if (etapaDto.Id == 0)
                        {
                            var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
                            novaEtapa.ObraId = obraExistente.Id;
                            await _obraEtapaRepo.AddAsync(novaEtapa);
                            foreach (var itemDto in etapaDto.Itens)
                            {
                                var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                                novoItem.ObraEtapaId = novaEtapa.Id;
                                await _obraItemEtapaRepo.AddAsync(novoItem);
                            }
                        }
                        else
                        {
                            var etapaExistente = etapasAtuais.FirstOrDefault(e => e.Id == etapaDto.Id);
                            if (etapaExistente != null)
                            {
                                AtualizarCamposEtapa(etapaExistente, etapaDto);
                                await _obraEtapaRepo.UpdateAsync(etapaExistente);

                                var itensAtuais = await _obraItemEtapaRepo.GetByEtapaIdAsync(etapaExistente.Id);
                                var itensRemovidos = itensAtuais
                                    .Where(i => !etapaDto.Itens.Any(dtoI => dtoI.Id == i.Id))
                                    .ToList();
                                foreach (var itemRemovido in itensRemovidos)
                                    await _obraItemEtapaRepo.RemoveAsync(itemRemovido);

                                foreach (var itemDto in etapaDto.Itens)
                                {
                                    if (itemDto.Id == 0)
                                    {
                                        var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                                        novoItem.ObraEtapaId = etapaExistente.Id;
                                        await _obraItemEtapaRepo.AddAsync(novoItem);
                                    }
                                    else
                                    {
                                        var itemExistente = itensAtuais.FirstOrDefault(i => i.Id == itemDto.Id);
                                        if (itemExistente != null)
                                        {
                                            AtualizarCamposItem(itemExistente, itemDto);
                                            await _obraItemEtapaRepo.UpdateAsync(itemExistente);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Etapa não encontrada no banco, mesmo com Id preenchido.
                                // Trata como uma nova etapa para garantir inclusão.

                                var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
                                novaEtapa.Id = 0; // força inserção como nova
                                novaEtapa.ObraId = obraExistente.Id;

                                await _obraEtapaRepo.AddAsync(novaEtapa);

                                foreach (var itemDto in etapaDto.Itens)
                                {
                                    if (obraDto.Id == 0)
                                    {
                                        foreach (var etapa in obraDto.Etapas)
                                        {
                                            etapa.Id = 0;
                                            foreach (var item in etapa.Itens)
                                                item.Id = 0;
                                        }

                                        var novaObra = _mapper.Map<Obra>(obraDto);
                                        novaObra.ProjetoId = projeto.Id;
                                        novaObra.UsuarioCadastro = UsuarioLogado;
                                        novaObra.DataHoraCadastro = DateTime.Now;
                                        await _obraRepo.AddAsync(novaObra);
                                    }

                                }
                            }
                        }
                    }
                    #endregion

                    #region Retrabalho

                    var retrabalhosAtuais = await _obraRetrabalhoRepo.GetByObraIdAsync(obraExistente.Id);
                    var retrabalhosRemovidos = retrabalhosAtuais
                        .Where(r => !obraDto.Retrabalhos.Any(dtoR => dtoR.Id == r.Id))
                        .ToList();

                    if (retrabalhosRemovidos.Count > 0)
                    {
                        foreach (var retrabalhoRemovido in retrabalhosRemovidos)
                            await _obraRetrabalhoRepo.RemoveAsync(retrabalhoRemovido);
                    }

                    foreach (var retrabalhoDto in obraDto.Retrabalhos)
                    {
                        if (retrabalhoDto.Id == 0)
                        {
                            var novo = _mapper.Map<ObraRetrabalho>(retrabalhoDto);
                            novo.ObraId = obraExistente.Id;
                            novo.UsuarioCadastro = UsuarioLogado;
                            novo.DataHoraCadastro = DateTime.Now;
                            await _obraRetrabalhoRepo.AddAsync(novo);
                        }
                        else
                        {
                            var retrabalhoExistente = retrabalhosAtuais.FirstOrDefault(r => r.Id == retrabalhoDto.Id);
                            if (retrabalhoExistente != null)
                            {
                                AtualizarCamposRetrabalho(retrabalhoExistente, retrabalhoDto);
                                await _obraRetrabalhoRepo.UpdateAsync(retrabalhoExistente);
                            }
                        }
                    }
                    #endregion

                    #region Pendencia

                    var pendenciasAtuais = await _obraPendenciaRepo.GetByObraIdAsync(obraExistente.Id);
                    var pendenciasRemovidos = pendenciasAtuais
                        .Where(r => !obraDto.Pendencias.Any(dtoR => dtoR.Id == r.Id))
                        .ToList();

                    if (pendenciasRemovidos.Count > 0)
                    {
                        foreach (var pendenciaRemovido in pendenciasRemovidos)
                            await _obraPendenciaRepo.RemoveAsync(pendenciaRemovido);
                    }

                    foreach (var pendenciaDto in obraDto.Pendencias)
                    {
                        if (pendenciaDto.Id == 0)
                        {
                            var novo = _mapper.Map<ObraPendencia>(pendenciaDto);
                            novo.ObraId = obraExistente.Id;
                            novo.UsuarioCadastro = UsuarioLogado;
                            novo.DataHoraCadastro = DateTime.Now;
                            await _obraPendenciaRepo.AddAsync(novo);
                        }
                        else
                        {
                            var pendenciaExistente = pendenciasAtuais.FirstOrDefault(r => r.Id == pendenciaDto.Id);
                            if (pendenciaExistente != null)
                            {
                                AtualizarCamposPendencia(pendenciaExistente, pendenciaDto);
                                await _obraPendenciaRepo.UpdateAsync(pendenciaExistente);
                            }
                        }
                    }

                    #endregion

                    #region Documentos
                    var documentosAtuais = await _obraDocumentoRepo.GetByObraIdAsync(obraExistente.Id);

                    // Remover os documentos que não estão mais presentes
                    var idsDocRemover = documentosAtuais
                        .Where(d => !obraDto.Documentos.Any(dd => dd.Id == d.Id))
                        .ToList();

                    if (idsDocRemover.Count > 0)
                    {
                        foreach (var doc in idsDocRemover)
                            await _obraDocumentoRepo.RemoveAsync(doc);
                    }

                    // Adicionar ou atualizar
                    foreach (var docDto in obraDto.Documentos)
                    {
                        if (docDto.Id == 0)
                        {
                            var novoDoc = _mapper.Map<ObraDocumento>(docDto);
                            novoDoc.ObraId = obraExistente.Id;
                            novoDoc.UsuarioCadastro = UsuarioLogado;
                            novoDoc.DataHoraCadastro = DateTime.Now;
                            await _obraDocumentoRepo.AddAsync(novoDoc);
                        }
                    }
                    #endregion

                    #region Imagens
                    var ImagensAtuais = await _obraImagemRepo.GetByObraIdAsync(obraExistente.Id);

                    // Remover os Imagens que não estão mais presentes
                    var idsImgRemover = ImagensAtuais
                        .Where(d => !obraDto.Imagens.Any(dd => dd.Id == d.Id))
                        .ToList();

                    if (idsImgRemover.Count > 0)
                    {
                        foreach (var Img in idsImgRemover)
                            await _obraImagemRepo.RemoveAsync(Img);
                    }

                    // Adicionar ou atualizar
                    foreach (var ImgDto in obraDto.Imagens)
                    {
                        if (ImgDto.Id == 0)
                        {
                            var novoImg = _mapper.Map<ObraImagem>(ImgDto);
                            novoImg.ObraId = obraExistente.Id;
                            novoImg.UsuarioCadastro = UsuarioLogado;
                            novoImg.DataHoraCadastro = DateTime.Now;
                            await _obraImagemRepo.AddAsync(novoImg);
                        }
                    }
                    #endregion



                }
            }
        }

        _mapper.Map(dto, projeto);
        _repo.AnexarEntidade(projeto);
        await _repo.UpdateAsync(projeto);
        await _auditoriaService.RegistrarAtualizacaoAsync(_mapper.Map<ProjetoDto>(projeto), dto, UsuarioLogado);
    }
    public async Task RemoverAsync(long id)
    {
        var projeto = await _repo.GetByIdAsync(id);
        if (projeto == null) return;

        await _repo.RemoveAsync(projeto);
        await _auditoriaService.RegistrarExclusaoAsync(_mapper.Map<ProjetoDto>(projeto), UsuarioLogado);
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosAgendadosAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosAgendados = projetos.Where(x => x.Status == StatusProjeto.Agendado);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosAgendados);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosEmPlanejamentoAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosEmPlanejamento = projetos.Where(x => x.Status == StatusProjeto.EmPlanejamento);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosEmPlanejamento);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosEmAndamentoAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosEmAndamento = projetos.Where(x => x.Status == StatusProjeto.EmAndamento);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosEmAndamento);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosConcluidosAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosConcluidos = projetos.Where(x => x.Status == StatusProjeto.Concluido);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosConcluidos);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosCanceladosAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosCancelados = projetos.Where(x => x.Status == StatusProjeto.Cancelado);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosCancelados);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }
    public async Task<IEnumerable<ProjetoDto>> ObterTodosPausadosAsync()
    {
        var projetos = await _repo.GetAllAsync();
        var projetosPausados = projetos.Where(x => x.Status == StatusProjeto.Pausado);
        var dtos = _mapper.Map<List<ProjetoDto>>(projetosPausados);

        foreach (var dto in dtos)
        {
            var obras = await _obraRepo.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }

}

