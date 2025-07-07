using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces; // Assumindo que IUnitOfWork está aqui
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class ProjetoService : IProjetoService
{
    // Apenas uma injeção para a unidade de trabalho, que contém todos os repositórios.
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjetoService(
        IUnitOfWork unitOfWork, // <- Injeção de múltiplos repositórios substituída pela Unit of Work
        IAuditoriaService auditoriaService,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string UsuarioLogado =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

    public async Task<IEnumerable<ProjetoDto>> ObterTodosAsync()
    {
        // Acesso ao repositório através da Unit of Work
        var projetos = await _unitOfWork.ProjetoRepository.GetAllAsync();
        var dtos = _mapper.Map<List<ProjetoDto>>(projetos);

        foreach (var dto in dtos)
        {
            var obras = await _unitOfWork.ObraRepository.GetByProjetoIdAsync(dto.Id);
            dto.Obras = _mapper.Map<List<ObraDto>>(obras);
        }

        return dtos;
    }

    public async Task<IEnumerable<ProjetoListDto>> ObterTodosListAsync()
    {
        var dtos = await _unitOfWork.ProjetoRepository.GetQueryable()
            .OrderBy(p => p.Nome)
            .Select(projeto => new ProjetoListDto
            {
                Id = projeto.Id,
                Nome = projeto.Nome,
                Status = projeto.Status,
                DataInicio = projeto.DataInicio != null ? DateOnly.FromDateTime(projeto.DataInicio) : null,
                DataFim = projeto.DataFim.HasValue ? DateOnly.FromDateTime(projeto.DataFim.Value) : null,
                ProgressoProjeto = projeto.Obras.Any()
                    ? (int)projeto.Obras.Average(obra =>
                        obra.Etapas.SelectMany(etapa => etapa.Itens).Any()
                        ? ((double)obra.Etapas.SelectMany(etapa => etapa.Itens).Count(item => item.Concluido) /
                           obra.Etapas.SelectMany(etapa => etapa.Itens).Count()) * 100
                        : 0)
                    : 0
            })
            .ToListAsync();

        return dtos;
    }

    public async Task<ProjetoDto?> ObterPorIdAsync(long id)
    {
        var projeto = await _unitOfWork.ProjetoRepository.FindOneWithIncludesAsync(p => p.Id == id, p => p.Endereco, p => p.Obras); 
        if (projeto == null) return null;
        return _mapper.Map<ProjetoDto>(projeto);
    }

    public async Task<ProjetoDto> CriarAsync(ProjetoDto dto)
    {
        var projeto = _mapper.Map<Projeto>(dto);

        projeto.UsuarioCadastro = UsuarioLogado;
        projeto.DataHoraCadastro = DateTime.Now;

        // Lógica para Endereço (Criação)
        if (!string.IsNullOrWhiteSpace(dto.CEP))
        {
            var enderecoEntity = _mapper.Map<Endereco>(dto);
            _unitOfWork.EnderecoRepository.Add(enderecoEntity);
            projeto.Endereco = enderecoEntity;
        }

        foreach (var obra in projeto.Obras)
        {
            obra.UsuarioCadastro = UsuarioLogado;
            obra.DataHoraCadastro = DateTime.Now;

            if (obra.ResponsavelObraId == 0)
            {
                obra.ResponsavelObraId = null;
            }

            if (obra.ResponsavelObraId.HasValue)
            {
                // Validação utilizando o repositório da Unit of Work
                var responsavelExiste = await _unitOfWork.FuncionarioRepository.ExistsAsync(x => x.Id == obra.ResponsavelObraId);
                if (!responsavelExiste)
                {
                    throw new InvalidOperationException($"O responsável pela obra com ID {obra.ResponsavelObraId.Value} não existe.");
                }
            }

            foreach (var etapa in obra.Etapas)
            {
                etapa.UsuarioCadastro = UsuarioLogado;
                etapa.DataHoraCadastro = DateTime.Now;

                foreach (var item in etapa.Itens)
                {
                    item.UsuarioCadastro = UsuarioLogado;
                    item.DataHoraCadastro = DateTime.Now;
                }
            }
        }

        // 1. Adiciona a entidade raiz ao contexto. O EF Core rastreia o grafo de objetos.
        _unitOfWork.ProjetoRepository.Add(projeto);

        // 2. Comita todas as mudanças para o banco de dados em uma única transação.
        await _unitOfWork.CommitAsync();

        // 3. Registra a auditoria após a confirmação da transação.
        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<ProjetoDto>(projeto), UsuarioLogado);

        return _mapper.Map<ProjetoDto>(projeto);
    }

    public async Task AtualizarAsync(ProjetoDto dto)
    {
        var projeto = await _unitOfWork.ProjetoRepository.FindOneWithIncludesAsync( p => p.Id == dto.Id, p => p.Endereco, p => p.Obras);
        if (projeto == null) return;

        var dtoOriginal = _mapper.Map<ProjetoDto>(projeto);

        // Mapeia os dados do DTO para a entidade principal
        _mapper.Map(dto, projeto);
        projeto.UsuarioCadastro = UsuarioLogado;
        projeto.DataHoraCadastro = DateTime.Now;

        // 3. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
        if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
        {
            if (projeto.Endereco == null) // Se o cliente NÃO tinha endereço ANTES
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço
                projeto.Endereco = novoEndereco; // Associa o novo endereço ao cliente
            }
            else // Se o cliente JÁ tinha endereço
            {
                _mapper.Map(dto, projeto.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                                                            // Não é preciso _enderecoRepository.Update(clienteToUpdate.Endereco);
                                                            // O EF detectará as mudanças automaticamente porque ele já está rastreado.
            }
        }
        else // Se o DTO NÃO tem CEP, e o cliente TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
        {
            if (projeto.Endereco != null)
            {
                _unitOfWork.EnderecoRepository.Remove(projeto.Endereco); // Marca o endereço para remoção
                projeto.Endereco = null; // Desvincula o endereço do cliente
                projeto.EnderecoId = null; // Garante que a FK também seja nullificada
            }
        }

        if (projeto.EnderecoId == null)
        {
            var novoEndereco = _mapper.Map<Endereco>(dto);
            _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
            projeto.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
        }

        _unitOfWork.ProjetoRepository.Update(projeto);

        // Lógica para sincronizar as obras (Adicionar, Atualizar, Remover)
        await SincronizarObrasAsync(dto, projeto);

        // Comita todas as alterações (do projeto e das obras) em uma única transação.
        await _unitOfWork.CommitAsync();

        // Registra a auditoria após a transação ser bem-sucedida.
        await _auditoriaService.RegistrarAtualizacaoAsync(dtoOriginal, dto, UsuarioLogado);
    }

    private async Task SincronizarObrasAsync(ProjetoDto dto, Projeto projeto)
    {
        var obrasDtoIds = dto.Obras.Select(o => o.Id).ToHashSet();

        // Remover obras que não estão mais no DTO
        var obrasParaRemover = projeto.Obras.Where(o => !obrasDtoIds.Contains(o.Id)).ToList();
        foreach (var obra in obrasParaRemover)
        {
            _unitOfWork.ObraRepository.Remove(obra); // A remoção será comitada no final
        }

        // Atualizar obras existentes e adicionar novas
        foreach (var obraDto in dto.Obras)
        {
            var obraExistente = projeto.Obras.FirstOrDefault(o => o.Id == obraDto.Id);

            if (obraExistente != null)
            {
                // Atualizar
                _mapper.Map(obraDto, obraExistente);
                obraExistente.UsuarioCadastro = UsuarioLogado;
                obraExistente.DataHoraCadastro = DateTime.Now;
                _unitOfWork.ObraRepository.Update(obraExistente);
            }
            else
            {
                // Adicionar
                var novaObra = _mapper.Map<Obra>(obraDto);
                novaObra.ProjetoId = projeto.Id;
                novaObra.UsuarioCadastro = UsuarioLogado;
                novaObra.DataHoraCadastro = DateTime.Now;
                _unitOfWork.ObraRepository.Add(novaObra);
            }
        }
    }


    public async Task RemoverAsync(long id)
    {
        var projeto = await _unitOfWork.ProjetoRepository.GetByIdAsync(id);
        if (projeto == null) return;

        var dtoParaAuditoria = _mapper.Map<ProjetoDto>(projeto);
        
        // A remoção em cascata (se configurada no EF Core) cuidará das entidades filhas.
        _unitOfWork.ProjetoRepository.Remove(projeto);

        // Comita a remoção em uma única transação.
        await _unitOfWork.CommitAsync();

        // Registra a auditoria após a remoção bem-sucedida.
        await _auditoriaService.RegistrarExclusaoAsync(dtoParaAuditoria, UsuarioLogado);
    }

    // Os métodos de consulta por status podem ser otimizados para evitar o N+1
    // e para filtrar no banco de dados.

    private async Task<IEnumerable<ProjetoDto>> ObterProjetosPorStatusAsync(StatusProjeto status)
    {
        // Filtra no banco e inclui as Obras para evitar o problema N+1.
        var projetos = await _unitOfWork.ProjetoRepository.GetQueryable()
            .Where(p => p.Status == status)
            .Include(p => p.Obras) // Eager loading
            .ToListAsync();

        return _mapper.Map<List<ProjetoDto>>(projetos);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosAgendadosAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.Agendado);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosEmPlanejamentoAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.EmPlanejamento);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosEmAndamentoAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.EmAndamento);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosConcluidosAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.Concluido);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosCanceladosAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.Cancelado);
    }

    public async Task<IEnumerable<ProjetoDto>> ObterTodosPausadosAsync()
    {
        return await ObterProjetosPorStatusAsync(StatusProjeto.Pausado);
    }

    // Os métodos privados para atualização de campos não são mais necessários aqui,
    // pois a lógica de sincronização foi centralizada em `AtualizarAsync` e `SincronizarObrasAsync`.
    // O AutoMapper cuida da maior parte do mapeamento.
}