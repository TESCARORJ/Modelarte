// Arquivo: Application/Services/UsuarioLogadoService.cs

using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

public class UsuarioLogadoService : IUsuarioLogadoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    // Injeta a FACTORY, não a UnitOfWork
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<UnitOfWork> _unitOfWorkLogger;

    public UsuarioLogadoService(
        IHttpContextAccessor httpContextAccessor,
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILogger<UnitOfWork> unitOfWorkLogger)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextFactory = contextFactory;
        _unitOfWorkLogger = unitOfWorkLogger;
    }

    public async Task<Usuario?> ObterUsuarioAtualAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return null;

        // Cria uma UnitOfWork nova e de curta duração para esta operação específica.
        // O 'using' garante que o DbContext interno seja descartado ao final.
        await using var unitOfWork = new UnitOfWork(_contextFactory, _unitOfWorkLogger);

        // Garante que o usuário é obtido sem ser rastreado pelo DbContext.
        // Isso é crucial se o UsuarioRepository não usar AsNoTracking por padrão.
        // E inclua o PerfilUsuario para o mapeamento posterior, se necessário.
        return await unitOfWork.UsuarioRepository.ObterPorEmailComPerfilAsync(email);
    }
}