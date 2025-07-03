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
    private readonly IPerfilUsuarioRepository _repo;
    private readonly ILogAuditoriaRepository _logRepo;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PerfilUsuarioService(
        IPerfilUsuarioRepository repo,
        ILogAuditoriaRepository logRepo,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _repo = repo;
        _logRepo = logRepo;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string UsuarioLogado =>
        _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

    public async Task<IEnumerable<PerfilUsuarioDto>> ObterTodosAsync()
    {
        var perfilUsuarios = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<PerfilUsuarioDto>>(perfilUsuarios);
    }

    public async Task<PerfilUsuarioDto?> ObterPorIdAsync(long id)
    {
        var perfilUsuario = await _repo.GetByIdAsync(id);
        return perfilUsuario == null ? null : _mapper.Map<PerfilUsuarioDto>(perfilUsuario);
    }

    public async Task CriarAsync(PerfilUsuarioDto dto)
    {
        var entity = _mapper.Map<PerfilUsuario>(dto);
        entity.DataHoraCadastro = DateTime.Now;
        entity.UsuarioCadastro = UsuarioLogado;

        _repo.Add(entity);

        await _logRepo.RegistrarAsync(new LogAuditoria
        {
            Usuario = UsuarioLogado,
            Entidade = nameof(PerfilUsuario),
            Acao = "Criado",
            Descricao = $"PerfilUsuario '{entity.Nome}' criado",
            DadosAtuais = JsonSerializer.Serialize(entity)
        });
    }

    public async Task AtualizarAsync(PerfilUsuarioDto dto)
    {
        var entityAntigo = await _repo.GetByIdAsync(dto.Id);
        if (entityAntigo == null) return;

        var entityNovo = _mapper.Map<PerfilUsuario>(dto);
        entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
        entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

        _repo.Update(entityNovo);

        await _logRepo.RegistrarAsync(new LogAuditoria
        {
            Usuario = UsuarioLogado,
            Entidade = nameof(PerfilUsuario),
            Acao = "Atualizado",
            Descricao = $"PerfilUsuario '{entityNovo.Nome}' atualizado",
            DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
            DadosAtuais = JsonSerializer.Serialize(entityNovo)
        });
    }

    public async Task RemoverAsync(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return;

        _repo.Remove(entity);

        await _logRepo.RegistrarAsync(new LogAuditoria
        {
            Usuario = UsuarioLogado,
            Entidade = nameof(PerfilUsuario),
            Acao = "Excluído",
            Descricao = $"PerfilUsuario '{entity.Nome}' removido",
            DadosAnteriores = JsonSerializer.Serialize(entity)
        });
    }

   
}
