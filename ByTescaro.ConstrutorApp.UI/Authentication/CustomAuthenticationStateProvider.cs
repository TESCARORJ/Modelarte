using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ByTescaro.ConstrutorApp.UI.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor, IUsuarioLogadoService usuarioLogadoService)
        {
            _httpContextAccessor = httpContextAccessor;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var usuario = await _usuarioLogadoService.ObterUsuarioAtualAsync();

            if (usuario == null)
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymous);
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.PerfilUsuario.Nome)
            }, "CustomAuth");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(Usuario usuario)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.PerfilUsuario.Nome)
            }, "CustomAuth");

            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}
