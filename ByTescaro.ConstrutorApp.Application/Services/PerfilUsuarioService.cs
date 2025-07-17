using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class PerfilUsuarioService : IPerfilUsuarioService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public PerfilUsuarioService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _auditoriaService = auditoriaService;
        _usuarioLogadoService = usuarioLogadoService;
    }

    public async Task<IEnumerable<PerfilUsuarioDto>> ObterTodosAsync()
    {
        var perfilUsuarios = await _unitOfWork.PerfilUsuarioRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PerfilUsuarioDto>>(perfilUsuarios);
    }

    public async Task<PerfilUsuarioDto?> ObterPorIdAsync(long id)
    {
        var perfilUsuario = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(id);
        return perfilUsuario == null ? null : _mapper.Map<PerfilUsuarioDto>(perfilUsuario);
    }

    public async Task CriarAsync(PerfilUsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = _mapper.Map<PerfilUsuario>(dto);
        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastroId = usuarioLogadoId;

        _unitOfWork.PerfilUsuarioRepository.Add(entity);

        await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

    public async Task AtualizarAsync(PerfilUsuarioDto dto)
    {
        // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
        // Essa instância 'perfilUsuarioAntigoParaAuditoria' NÃO será modificada e representa o estado original.
        var perfilUsuarioAntigoParaAuditoria = await _unitOfWork.PerfilUsuarioRepository.GetByIdNoTrackingAsync(dto.Id);

        if (perfilUsuarioAntigoParaAuditoria == null)
        {
            // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
            throw new KeyNotFoundException($"Perfil de Usuário com ID {dto.Id} não encontrado para auditoria.");
        }

        // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
        // Essa instância 'perfilUsuarioParaAtualizar' é a que o EF Core está monitorando
        // e que terá suas propriedades alteradas e salvas no banco de dados.
        var perfilUsuarioParaAtualizar = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(dto.Id);

        if (perfilUsuarioParaAtualizar == null)
        {
            // Isso deve ser raro se 'perfilUsuarioAntigoParaAuditoria' foi encontrado,
            // mas é uma boa verificação de segurança para o fluxo de atualização.
            throw new KeyNotFoundException($"Perfil de Usuário com ID {dto.Id} não encontrado para atualização.");
        }

        // 3. Mapeie as propriedades do DTO para a entidade 'perfilUsuarioParaAtualizar' (a rastreada).
        // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
        _mapper.Map(dto, perfilUsuarioParaAtualizar);

        // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
        // Eles vêm da entidade original não modificada.
        // CUIDADO: Você tinha entityNovo.UsuarioCadastroId = entityAntigo.Id; na sua versão original.
        // O correto aqui é perfilUsuarioAntigoParaAuditoria.UsuarioCadastroId (o ID do usuário que CADASTRou).
        perfilUsuarioParaAtualizar.UsuarioCadastroId = perfilUsuarioAntigoParaAuditoria.UsuarioCadastroId;
        perfilUsuarioParaAtualizar.DataHoraCadastro = perfilUsuarioAntigoParaAuditoria.DataHoraCadastro;

        // O método .Update() no repositório é geralmente redundante se a entidade já está
        // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
        // _unitOfWork.PerfilUsuarioRepository.Update(perfilUsuarioParaAtualizar);

        // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
        // 'perfilUsuarioAntigoParaAuditoria' tem os dados ANTES da mudança.
        // 'perfilUsuarioParaAtualizar' tem os dados DEPOIS da mudança.
        await _auditoriaService.RegistrarAtualizacaoAsync(perfilUsuarioAntigoParaAuditoria, perfilUsuarioParaAtualizar, usuarioLogadoId);

        // 5. Salve TODAS as alterações no banco de dados em uma única transação.
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoverAsync(long id)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = await _unitOfWork.PerfilUsuarioRepository.GetByIdAsync(id);
        if (entity == null) return;

        _unitOfWork.PerfilUsuarioRepository.Remove(entity);
        await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
        await _unitOfWork.CommitAsync();
    }

   
}
