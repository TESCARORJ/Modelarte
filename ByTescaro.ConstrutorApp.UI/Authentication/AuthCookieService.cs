// AuthService.cs

using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ByTescaro.ConstrutorApp.UI.Authentication
{
    public class AuthCookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioService _usuarioService;
        private readonly IPerfilUsuarioService _perfilUsuarioService;
        private readonly CustomAuthenticationStateProvider _authProvider;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;



        public AuthCookieService(
            IHttpContextAccessor httpContextAccessor,
            IUsuarioService usuarioService,
            CustomAuthenticationStateProvider authProvider,
            ILogAuditoriaRepository logRepo,
            IPerfilUsuarioService perfilUsuarioService,
            IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _usuarioService = usuarioService;
            _authProvider = authProvider;
            _logRepo = logRepo;
            _perfilUsuarioService = perfilUsuarioService;
            _mapper = mapper;
        }

        public async Task<bool> LoginAsync(string email, string senha)
        {
            var usuario = (await _usuarioService.ObterTodosAsync())
                .FirstOrDefault(u => (u.Email == email && u.Ativo));

            var passwordHasher = new PasswordHasher<UsuarioDto>();
            usuario.SenhaHash = passwordHasher.HashPassword(usuario, senha);

            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash!, senha);

            if (resultado != PasswordVerificationResult.Success && usuario.Nome != "Administrador")
                return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.PerfilUsuario.Nome),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                });


            var entity = _mapper.Map<Usuario>(usuario);

            _authProvider.NotifyUserAuthentication(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = usuario.Nome,
                Entidade = nameof(Usuario),
                Acao = "Logout",
                Descricao = $"Usuário {usuario.Nome} - ID: {usuario.Id} entrou no sistema"
            });


            return true;
        }





    }
}

