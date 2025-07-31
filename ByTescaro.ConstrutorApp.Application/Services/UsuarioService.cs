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
        var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        
        var usuarioAntigoParaAuditoria = await _unitOfWork.UsuarioRepository.GetByIdNoTrackingAsync(dto.Id);

        if (usuarioAntigoParaAuditoria == null)
        {
            
            throw new KeyNotFoundException($"Usuário com ID {dto.Id} não encontrado para auditoria.");
        }

       
        var usuarioParaAtualizar = await _unitOfWork.UsuarioRepository.GetByIdAsync(dto.Id);

        if (usuarioParaAtualizar == null)
        {
          
            throw new KeyNotFoundException($"Usuário com ID {dto.Id} não encontrado para atualização.");
        }

        usuarioParaAtualizar.TipoEntidade = TipoEntidadePessoa.Usuario;
        _mapper.Map(dto, usuarioParaAtualizar);

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var hasher = new PasswordHasher<Usuario>();
           
            var novaSenhaHash = hasher.HashPassword(usuarioParaAtualizar, dto.Senha);

            if (usuarioParaAtualizar.SenhaHash != novaSenhaHash)
            {
                usuarioParaAtualizar.SenhaHash = novaSenhaHash;
            }
        }

        
        usuarioParaAtualizar.UsuarioCadastroId = usuarioAntigoParaAuditoria.UsuarioCadastroId;
        usuarioParaAtualizar.DataHoraCadastro = usuarioAntigoParaAuditoria.DataHoraCadastro;


        _unitOfWork.UsuarioRepository.Update(usuarioParaAtualizar);

        await _unitOfWork.CommitAsync();
        await _auditoriaService.RegistrarAtualizacaoAsync(usuarioAntigoParaAuditoria, usuarioParaAtualizar, usuarioLogadoId);

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

        await _unitOfWork.CommitAsync();
        await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

    }

  
}
