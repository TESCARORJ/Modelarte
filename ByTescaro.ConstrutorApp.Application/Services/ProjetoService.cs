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
    private readonly IFuncionarioRepository _funcionarioRepo;


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
        IObraInsumoListaRepository obraInsumoListaRepo,
        IFuncionarioRepository funcionarioRepo)
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
        _funcionarioRepo = funcionarioRepo;
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
        // 1. Mapeie o DTO completo para a entidade Projeto.
        // O AutoMapper se encarregará de mapear a coleção de Obras também.
        var projeto = _mapper.Map<Projeto>(dto);

        projeto.UsuarioCadastro = UsuarioLogado;
        projeto.DataHoraCadastro = DateTime.Now;

        // 2. Itere sobre as obras para definir as propriedades de auditoria e a FK manualmente.
        foreach (var obra in projeto.Obras)
        {
            obra.ProjetoId = projeto.Id; // Esta linha não é necessária se o mapeamento estiver correto, mas pode ser útil para clareza
            obra.UsuarioCadastro = UsuarioLogado;
            obra.DataHoraCadastro = DateTime.Now;

            // Validação crucial para o ResponsavelObraId, como você já implementou
            if (obra.ResponsavelObraId == 0)
            {
                obra.ResponsavelObraId = null;
            }

            if (obra.ResponsavelObraId.HasValue)
            {
                var responsavelExiste = await _funcionarioRepo.ExistsAsync(obra.ResponsavelObraId.Value);
                if (!responsavelExiste)
                {
                    throw new InvalidOperationException($"O responsável pela obra com ID {obra.ResponsavelObraId.Value} não existe.");
                }
            }

            // Itere sobre as etapas para definir as propriedades de auditoria
            foreach (var etapa in obra.Etapas)
            {
                etapa.UsuarioCadastro = UsuarioLogado;
                etapa.DataHoraCadastro = DateTime.Now;
                etapa.ObraId = obra.Id; // Vincula a etapa à obra

                // Itere sobre os itens da etapa
                foreach (var item in etapa.Itens)
                {
                    item.UsuarioCadastro = UsuarioLogado;
                    item.DataHoraCadastro = DateTime.Now;
                    item.ObraEtapaId = etapa.Id; // Vincula o item à etapa
                }
            }
        }

        // 3. Adicione a entidade raiz (Projeto) ao repositório.
        // O EF Core irá rastrear todas as entidades relacionadas (Obras, Etapas, Itens)
        // porque elas fazem parte do grafo de objetos.
        await _repo.AddAsync(projeto); // Esta chamada irá salvar todo o grafo em uma transação.

        // 4. Registre a auditoria após a operação de salvamento bem-sucedida.
        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<ProjetoDto>(projeto), UsuarioLogado);

        // 5. Retorne o DTO mapeado.
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

