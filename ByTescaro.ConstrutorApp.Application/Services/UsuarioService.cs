using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IMapper _mapper;
    private readonly IUsuarioLogadoService _usuarioLogadoService;


    public UsuarioService(IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _auditoriaService = auditoriaService;
        _usuarioLogadoService = usuarioLogadoService;
        _unitOfWork = unitOfWork;
    }



    public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync()
    {
        var usuarios = await _unitOfWork.UsuarioRepository.FindAllWithIncludesAsync(x => x.TipoEntidade == TipoEntidadePessoa.Usuario, x => x.PerfilUsuario);

        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

    }

    public async Task<IEnumerable<UsuarioDto>> ObterTodosAtivosAsync()
    {
        var usuarios = await _unitOfWork.UsuarioRepository.FindAllWithIncludesAsync(x => x.TipoEntidade == TipoEntidadePessoa.Usuario && x.Ativo == true, x => x.PerfilUsuario);

        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

    }

    public async Task<UsuarioDto?> ObterPorIdAsync(long id)
    {
        var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    // EM UsuarioService.cs
    public async Task CriarAsync(UsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        // 1. Criar/Obter Endereco a partir do DTO
        Endereco? endereco = null;
        if (!string.IsNullOrEmpty(dto.Logradouro) ||
            !string.IsNullOrEmpty(dto.Numero) ||
            !string.IsNullOrEmpty(dto.Bairro) ||
            !string.IsNullOrEmpty(dto.Cidade) ||
            !string.IsNullOrEmpty(dto.Estado) ||
            !string.IsNullOrEmpty(dto.UF) ||
            !string.IsNullOrEmpty(dto.CEP))
        {
            endereco = _mapper.Map<Endereco>(dto); // Mapeia UsuarioDto (plano) para Endereco
            _unitOfWork.EnderecoRepository.Add(endereco); // Adiciona o Endereço ao contexto para ser salvo
            await _unitOfWork.CommitAsync(); // Salva o Endereço para obter o Id antes de atribuir ao Usuário
        }

        // 2. Mapear UsuarioDto para Usuario
        var entity = _mapper.Map<Usuario>(dto);

        // 3. Associar o Endereco criado/obtido ao Usuario
        if (endereco != null)
        {
            entity.EnderecoId = endereco.Id;
            entity.Endereco = endereco; // Opcional, mas útil para navegação imediata
        }

        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastroId = usuarioLogadoId;
        entity.TipoEntidade = TipoEntidadePessoa.Usuario;

        var hasher = new PasswordHasher<Usuario>();
        entity.SenhaHash = hasher.HashPassword(entity, dto.Senha!);

        _unitOfWork.UsuarioRepository.Add(entity);
        await _unitOfWork.CommitAsync();
        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<UsuarioDto>(entity), usuarioLogadoId); // Mapeia para DTO de auditoria
    }


    public async Task AtualizarAsync(UsuarioDto dto)
    {
        // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
        // Essa instância 'usuarioAntigoParaAuditoria' NÃO será modificada e representa o estado original.
        var usuarioAntigoParaAuditoria = await _unitOfWork.UsuarioRepository.GetByIdNoTrackingAsync(dto.Id);

        if (usuarioAntigoParaAuditoria == null)
        {
            // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
            throw new KeyNotFoundException($"Usuário com ID {dto.Id} não encontrado para auditoria.");
        }

        // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
        // Essa instância 'usuarioParaAtualizar' é a que o EF Core está monitorando
        // e que terá suas propriedades alteradas e salvas no banco de dados.
        var usuarioParaAtualizar = await _unitOfWork.UsuarioRepository.GetByIdAsync(dto.Id);

        if (usuarioParaAtualizar == null)
        {
            // Isso deve ser raro se 'usuarioAntigoParaAuditoria' foi encontrado,
            // mas é uma boa verificação de segurança para o fluxo de atualização.
            throw new KeyNotFoundException($"Usuário com ID {dto.Id} não encontrado para atualização.");
        }

        // Garante que o campo discriminador (se usado) não seja alterado.
        usuarioParaAtualizar.TipoEntidade = TipoEntidadePessoa.Usuario;

        // 3. Mapear os demais dados do DTO para a entidade rastreada ('usuarioParaAtualizar').
        // Isso evita sobrescrever SenhaHash acidentalmente se o AutoMapper não for configurado para ignorá-lo.
        _mapper.Map(dto, usuarioParaAtualizar);

        // Só atualiza a senha se o campo foi preenchido com valor diferente
        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var hasher = new PasswordHasher<Usuario>();
            // Verifica se a nova senha é diferente da atual (hashing a nova e comparando com a hash salva)
            // Se SenhaHash for null ou vazio, o VerifyHashedPassword retorna Failed.
            // Para garantir que a senha seja realmente diferente, você pode comparar as hashes.
            var novaSenhaHash = hasher.HashPassword(usuarioParaAtualizar, dto.Senha);

            if (usuarioParaAtualizar.SenhaHash != novaSenhaHash) // Comparação de hashes
            {
                usuarioParaAtualizar.SenhaHash = novaSenhaHash;
            }
        }

        // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
        // Eles vêm da entidade original não modificada.
        usuarioParaAtualizar.UsuarioCadastroId = usuarioAntigoParaAuditoria.UsuarioCadastroId;
        usuarioParaAtualizar.DataHoraCadastro = usuarioAntigoParaAuditoria.DataHoraCadastro;


        // O método .Update() no repositório é geralmente redundante se a entidade já está
        // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
        _unitOfWork.UsuarioRepository.Update(usuarioParaAtualizar);

        // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
        // 'usuarioAntigoParaAuditoria' tem os dados ANTES da mudança.
        // 'usuarioParaAtualizar' tem os dados DEPOIS da mudança.
        await _auditoriaService.RegistrarAtualizacaoAsync(usuarioAntigoParaAuditoria, usuarioParaAtualizar, usuarioLogadoId);

        // 5. Salva TODAS as alterações no banco de dados em uma única transação.
        await _unitOfWork.CommitAsync();
    }


    public async Task InativarAsync(long id, string atualizadoPor)
    {
        var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        if (usuario == null) return;
        usuario.Ativo = false;
        _unitOfWork.UsuarioRepository.Update(usuario);

    }

    public async Task ExcluirAsync(long id)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        if (entity == null) return;

        _unitOfWork.UsuarioRepository.Remove(entity);

        await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        await _unitOfWork.CommitAsync();
    }

  
}
