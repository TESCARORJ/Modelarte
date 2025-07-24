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

        // 1. Crie a entidade Projeto a partir do DTO, MAS SEM as Obras filhas por enquanto.
        var projetoEntity = _mapper.Map<Projeto>(dto, opts => opts.Items["IgnoreObras"] = true); // Usaremos um truque com AutoMapper

        // 2. Defina os dados de auditoria do Projeto.
        projetoEntity.UsuarioCadastroId = usuarioLogadoId;
        projetoEntity.DataHoraCadastro = DateTime.Now;

        // 3. Adicione o Projeto ao contexto IMEDIATAMENTE.
        // Agora o EF Core está rastreando esta instância.
        _unitOfWork.ProjetoRepository.Add(projetoEntity);

        // 4. Lógica para Endereço (se houver)
        if (!string.IsNullOrWhiteSpace(dto.CEP))
        {
            var enderecoEntity = _mapper.Map<Endereco>(dto);
            _unitOfWork.EnderecoRepository.Add(enderecoEntity);
            projetoEntity.Endereco = enderecoEntity;
        }

        // 5. AGORA, crie e associe as Obras à instância de Projeto que já está sendo rastreada.
        foreach (var obraDto in dto.Obras)
        {
            var obraEntity = _mapper.Map<Obra>(obraDto);
            obraEntity.UsuarioCadastroId = usuarioLogadoId;
            obraEntity.DataHoraCadastro = DateTime.Now;

            // A MÁGICA: Adicione a nova obra DIRETAMENTE à coleção do projeto JÁ RASTREADO.
            // O EF Core automaticamente entende que esta obra pertence a este projeto e definirá o ProjetoId.
            projetoEntity.Obras.Add(obraEntity);
        }

        // 6. Salve tudo.
        await _unitOfWork.CommitAsync();

        // 7. Auditoria (agora com o serializador corrigido)
        await _auditoriaService.RegistrarCriacaoAsync(projetoEntity, usuarioLogadoId);

        return _mapper.Map<ProjetoDto>(projetoEntity);
    }

    public async Task AtualizarAsync(ProjetoDto dto)
    {
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        // 1. Busque a entidade 'Projeto' original COM INCLUDES (Endereco, Obras) e SEM RASTREAMENTO.
        // Esta é a CÓPIA para auditoria. Ela representa o estado ANTES da atualização e não será modificada.
        var projetoAntigoParaAuditoria = await _unitOfWork.ProjetoRepository
            .FindOneWithIncludesNoTrackingAsync(p => p.Id == dto.Id, p => p.Endereco, p => p.Obras); // NOVO MÉTODO NECESSÁRIO

        if (projetoAntigoParaAuditoria == null)
        {
            throw new KeyNotFoundException($"Projeto com ID {dto.Id} não encontrado para auditoria.");
        }

        // 2. Busque a entidade 'Projeto' que REALMENTE SERÁ ATUALIZADA, COM INCLUDES e COM RASTREAMENTO.
        // Esta é a entidade que o EF Core irá monitorar e que terá suas propriedades e coleções alteradas.
        var projetoParaAtualizar = await _unitOfWork.ProjetoRepository
            .FindOneWithIncludesAsync(p => p.Id == dto.Id, p => p.Endereco, p => p.Obras);

        if (projetoParaAtualizar == null)
        {
            throw new KeyNotFoundException($"Projeto com ID {dto.Id} não encontrado para atualização.");
        }

        // 3. Mapeie os dados do DTO para a entidade principal rastreada ('projetoParaAtualizar')
        // Campos de auditoria de criação ('UsuarioCadastroId', 'DataHoraCadastro') devem ser preservados
        // e NÃO devem ser sobrescritos pelo DTO ou por DateTime.Now em uma atualização.
        // Eles são do momento da CRIAÇÃO do registro.
        _mapper.Map(dto, projetoParaAtualizar);
        projetoParaAtualizar.UsuarioCadastroId = projetoAntigoParaAuditoria.UsuarioCadastroId; // Preserva o ID do usuário que criou
        projetoParaAtualizar.DataHoraCadastro = projetoAntigoParaAuditoria.DataHoraCadastro; // Preserva a data/hora de criação


        // 4. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco (relacionamento 1:1)
        if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
        {
            if (projetoParaAtualizar.Endereco == null) // Se o projeto NÃO tinha endereço ANTES
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                projetoParaAtualizar.Endereco = novoEndereco; // Associa o novo endereço ao projeto
                projetoParaAtualizar.EnderecoId = novoEndereco.Id; // Garante que a FK seja definida
            }
            else // Se o projeto JÁ tinha endereço, atualiza o existente
            {
                _mapper.Map(dto, projetoParaAtualizar.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                // O EF detectará as mudanças automaticamente porque ele já está rastreado.
            }
        }
        else // Se o DTO NÃO tem CEP, e o projeto TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
        {
            if (projetoParaAtualizar.Endereco != null)
            {
                _unitOfWork.EnderecoRepository.Remove(projetoParaAtualizar.Endereco); // Marca o endereço para remoção
                projetoParaAtualizar.Endereco = null; // Desvincula o endereço do projeto
                projetoParaAtualizar.EnderecoId = null; // Garante que a FK também seja nullificada
            }
        }

        // Nota: A sua condição original 'if (projeto.EnderecoId == null)' após a lógica acima é redundante
        // e pode levar à criação duplicada de endereços ou comportamento inesperado.
        // A lógica de criação/atualização/remoção já cobre todos os casos.

        // O _unitOfWork.ProjetoRepository.Update(projeto); é geralmente redundante aqui,
        // pois a entidade principal já está rastreada e suas propriedades foram modificadas.
        // O EF Core detectará as mudanças automaticamente.
        // _unitOfWork.ProjetoRepository.Update(projetoParaAtualizar);


        // 5. Lógica para sincronizar as obras (Adicionar, Atualizar, Remover) - Relacionamento 1:N
        // Implementar esta lógica de forma mais robusta é crucial para coleções.
        // O exemplo abaixo é uma abordagem comum:

        // Obras a serem removidas (existem no projeto, mas não no DTO)
        var obrasParaRemover = projetoParaAtualizar.Obras
            .Where(existingObra => !dto.Obras.Any(dtoObra => dtoObra.Id == existingObra.Id && dtoObra.Id != 0))
            .ToList();

        foreach (var obra in obrasParaRemover)
        {
            projetoParaAtualizar.Obras.Remove(obra);
            _unitOfWork.ObraRepository.Remove(obra); // Marcar para remoção explícita
        }

        // Obras a serem adicionadas ou atualizadas
        foreach (var obraDto in dto.Obras)
        {
            var existingObra = projetoParaAtualizar.Obras.FirstOrDefault(o => o.Id == obraDto.Id && o.Id != 0);

            if (existingObra == null) // Obra nova
            {
                var novaObra = _mapper.Map<Obra>(obraDto);
                novaObra.UsuarioCadastroId = usuarioLogadoId;
                novaObra.DataHoraCadastro = DateTime.Now;
                projetoParaAtualizar.Obras.Add(novaObra);
                // Não é necessário _unitOfWork.ObraRepository.Add(novaObra); aqui,
                // se 'projetoParaAtualizar' é o pai e está sendo rastreado,
                // o EF Core adicionará a nova obra quando o pai for salvo.
            }
            else // Obra existente, atualizar
            {
                _mapper.Map(obraDto, existingObra);
                // O EF Core detectará as mudanças automaticamente.
            }
        }

        // 6. Salva TODAS as alterações (Projeto, Endereço e Obras) em uma única transação.
        await _unitOfWork.CommitAsync();

        // 7. Registra a auditoria após a transação ser bem-sucedida.
        // Agora, 'projetoAntigoParaAuditoria' tem o estado "antes", e 'projetoParaAtualizar' tem o estado "depois".
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
        
        // A remoção em cascata (se configurada no EF Core) cuidará das entidades filhas.
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