using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces; // Assumindo que IUnitOfWork está aqui
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class ProjetoService : IProjetoService
{
    // Apenas uma injeção para a unidade de trabalho, que contém todos os repositórios.
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly IUsuarioLogadoService _usuarioLogadoService;


    public ProjetoService(
        IUnitOfWork unitOfWork, // <- Injeção de múltiplos repositórios substituída pela Unit of Work
        IAuditoriaService auditoriaService,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService)
    {
        _unitOfWork = unitOfWork;
        _auditoriaService = auditoriaService;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
    }


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
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? throw new InvalidOperationException("Usuário não autenticado para criar projeto.");

        var projetoEntity = _mapper.Map<Projeto>(dto, opts => opts.Items["IgnoreObras"] = true);

        projetoEntity.UsuarioCadastroId = usuarioLogadoId;
        projetoEntity.DataHoraCadastro = DateTime.Now;

        _unitOfWork.ProjetoRepository.Add(projetoEntity);

        if (!string.IsNullOrWhiteSpace(dto.CEP))
        {
            var enderecoEntity = _mapper.Map<Endereco>(dto);
            _unitOfWork.EnderecoRepository.Add(enderecoEntity);
            projetoEntity.Endereco = enderecoEntity;
        }

        foreach (var obraDto in dto.Obras)
        {
            var obraEntity = _mapper.Map<Obra>(obraDto);
            obraEntity.UsuarioCadastroId = usuarioLogadoId;
            obraEntity.DataHoraCadastro = DateTime.Now;

           
            projetoEntity.Obras.Add(obraEntity);
        }

        await _unitOfWork.CommitAsync();

        await _auditoriaService.RegistrarCriacaoAsync(projetoEntity, usuarioLogadoId);

        return _mapper.Map<ProjetoDto>(projetoEntity);
    }

    public async Task AtualizarAsync(ProjetoDto dto)
    {
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

       
        var projetoAntigoParaAuditoria = await _unitOfWork.ProjetoRepository
            .FindOneWithIncludesNoTrackingAsync(p => p.Id == dto.Id, p => p.Endereco, p => p.Obras);

        if (projetoAntigoParaAuditoria == null)
        {
            throw new KeyNotFoundException($"Projeto com ID {dto.Id} não encontrado para auditoria.");
        }

     
        var projetoParaAtualizar = await _unitOfWork.ProjetoRepository
            .FindOneWithIncludesAsync(p => p.Id == dto.Id, p => p.Endereco, p => p.Obras);

        if (projetoParaAtualizar == null)
        {
            throw new KeyNotFoundException($"Projeto com ID {dto.Id} não encontrado para atualização.");
        }

       
        _mapper.Map(dto, projetoParaAtualizar);
        projetoParaAtualizar.UsuarioCadastroId = projetoAntigoParaAuditoria.UsuarioCadastroId; 
        projetoParaAtualizar.DataHoraCadastro = projetoAntigoParaAuditoria.DataHoraCadastro; 


        if (!string.IsNullOrWhiteSpace(dto.CEP)) 
        {
            if (projetoParaAtualizar.Endereco == null) 
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(novoEndereco);
                projetoParaAtualizar.Endereco = novoEndereco;
                projetoParaAtualizar.EnderecoId = novoEndereco.Id; 
            }
            else
            {
                _mapper.Map(dto, projetoParaAtualizar.Endereco); 
            }
        }
        else 
        {
            if (projetoParaAtualizar.Endereco != null)
            {
                _unitOfWork.EnderecoRepository.Remove(projetoParaAtualizar.Endereco); 
                projetoParaAtualizar.Endereco = null;
                projetoParaAtualizar.EnderecoId = null;
            }
        }

       
        var obrasParaRemover = projetoParaAtualizar.Obras
            .Where(existingObra => !dto.Obras.Any(dtoObra => dtoObra.Id == existingObra.Id && dtoObra.Id != 0))
            .ToList();

        foreach (var obra in obrasParaRemover)
        {
            projetoParaAtualizar.Obras.Remove(obra);
            _unitOfWork.ObraRepository.Remove(obra); 
        }

        foreach (var obraDto in dto.Obras)
        {
            var existingObra = projetoParaAtualizar.Obras.FirstOrDefault(o => o.Id == obraDto.Id && o.Id != 0);

            if (existingObra == null)
            {
                var novaObra = _mapper.Map<Obra>(obraDto);
                novaObra.UsuarioCadastroId = usuarioLogadoId;
                novaObra.DataHoraCadastro = DateTime.Now;
                projetoParaAtualizar.Obras.Add(novaObra);
               
            }
            else 
            {
                _mapper.Map(obraDto, existingObra);
            }
        }

        await _unitOfWork.CommitAsync();
        await _auditoriaService.RegistrarAtualizacaoAsync(projetoAntigoParaAuditoria, projetoParaAtualizar, usuarioLogadoId);
    }
    private async Task SincronizarObrasAsync(ProjetoDto dto, Projeto projeto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

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
                obraExistente.UsuarioCadastroId = usuarioLogadoId;
                obraExistente.DataHoraCadastro = DateTime.Now;
                _unitOfWork.ObraRepository.Update(obraExistente);
            }
            else
            {
                // Adicionar
                var novaObra = _mapper.Map<Obra>(obraDto);
                novaObra.ProjetoId = projeto.Id;
                novaObra.UsuarioCadastroId = usuarioLogadoId;
                novaObra.DataHoraCadastro = DateTime.Now;
                _unitOfWork.ObraRepository.Add(novaObra);
            }
        }
    }


    public async Task RemoverAsync(long id)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var projeto = await _unitOfWork.ProjetoRepository.GetByIdAsync(id);
        if (projeto == null) return;

        var dtoParaAuditoria = _mapper.Map<ProjetoDto>(projeto);
        
        _unitOfWork.ProjetoRepository.Remove(projeto);

        // Comita a remoção em uma única transação.
        await _unitOfWork.CommitAsync();

        // Registra a auditoria após a remoção bem-sucedida.
        await _auditoriaService.RegistrarExclusaoAsync(dtoParaAuditoria, usuarioLogadoId);
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