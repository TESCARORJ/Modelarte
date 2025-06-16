using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;


    public UsuarioService(IHttpContextAccessor httpContextAccessor, IMapper mapper, IUsuarioRepository repo, IAuditoriaService auditoriaService)
    {
        _repo = repo;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _repo = repo;
        _auditoriaService = auditoriaService;
    }

    private string UsuarioLogado =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

    public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync()
    {
        var usuarios = await _repo.GetAllAsync();

        return _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

    }

    public async Task<UsuarioDto?> ObterPorIdAsync(long id)
    {
        var usuario = await _repo.GetByIdAsync(id);
        return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
    }

    public async Task CriarAsync(UsuarioDto dto)
    {
        var entity = _mapper.Map<Usuario>(dto);
        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastro = UsuarioLogado;

        var hasher = new PasswordHasher<Usuario>();
        entity.SenhaHash = hasher.HashPassword(entity, dto.Senha!);

        await _repo.AddAsync(entity);
        await _auditoriaService.RegistrarCriacaoAsync(_mapper.Map<UsuarioDto>(entity), UsuarioLogado);
    }


    public async Task AtualizarAsync(UsuarioDto dto)
    {
        var entityAntigo = await _repo.GetByIdAsync(dto.Id);
        if (entityAntigo == null) return;

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

        await _repo.UpdateAsync(entityAntigo);
        await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, _mapper.Map<Usuario>(dto), UsuarioLogado);
    }


    public async Task InativarAsync(long id, string atualizadoPor)
    {
        var usuario = await _repo.GetByIdAsync(id);
        if (usuario == null) return;
        usuario.Ativo = false;
        await _repo.UpdateAsync(usuario);

    }

    public async Task ExcluirAsync(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return;

        await _repo.RemoveAsync(entity);
        await _auditoriaService.RegistrarExclusaoAsync(entity, UsuarioLogado);
    }

  
}
