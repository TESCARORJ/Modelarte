// Arquivo: Application/Services/UsuarioLogadoService.cs

using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class UsuarioLogadoService : IUsuarioLogadoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    // Injeta a FACTORY, não a UnitOfWork
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UsuarioLogadoService(
        IHttpContextAccessor httpContextAccessor,
        IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _contextFactory = contextFactory;
    }

    public async Task<Usuario?> ObterUsuarioAtualAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        // Cria uma UnitOfWork nova e de curta duração para esta operação específica.
        // O 'using' garante que o DbContext interno seja descartado ao final.
        await using var unitOfWork = new UnitOfWork(_contextFactory);

        return await unitOfWork.UsuarioRepository.ObterPorEmailComPerfilAsync(email);
    }
}