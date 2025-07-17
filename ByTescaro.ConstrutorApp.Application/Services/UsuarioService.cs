using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
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

    public async Task<UsuarioDto?> ObterPorIdAsync(long id)
    {
        var usuario = await _unitOfWork.UsuarioRepository.GetByIdAsync(id);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task CriarAsync(UsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entity = _mapper.Map<Usuario>(dto);
        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastroId = usuarioLogadoId;
        entity.TipoEntidade = TipoEntidadePessoa.Usuario;


        var hasher = new PasswordHasher<Usuario>();
        entity.SenhaHash = hasher.HashPassword(entity, dto.Senha!);

        _unitOfWork.UsuarioRepository.Add(entity);
        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<UsuarioDto>(entity), usuarioLogadoId);

        await _unitOfWork.CommitAsync();

    }


    public async Task AtualizarAsync(UsuarioDto dto)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

        var entityAntigo = await _unitOfWork.UsuarioRepository.GetByIdAsync(dto.Id);
        if (entityAntigo == null) return;
        entityAntigo.TipoEntidade = TipoEntidadePessoa.Usuario;


        // Mapear os demais dados do DTO para a entidade antiga
        _mapper.Map(dto, entityAntigo); // Isso evita sobrescrever SenhaHash acidentalmente

        // Só atualiza a senha se o campo foi preenchido com valor diferente
        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var hasher = new PasswordHasher<Usuario>();
            var verifica = hasher.VerifyHashedPassword(entityAntigo, entityAntigo.SenhaHash, dto.Senha);

            if (verifica != PasswordVerificationResult.Success)
            {
                entityAntigo.SenhaHash = hasher.HashPassword(entityAntigo, dto.Senha);
            }
        }

        _unitOfWork.UsuarioRepository.Update(entityAntigo);
        await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, _mapper.Map<Usuario>(dto), usuarioLogadoId);
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
