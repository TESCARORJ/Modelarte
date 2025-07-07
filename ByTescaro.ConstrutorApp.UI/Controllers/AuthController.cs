using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs.Auth;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ByTescaro.ConstrutorApp.UI.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogAuditoriaRepository _logRepo;
        private IMapper _mapper;

        public AuthController(IUsuarioService usuarioService, ILogAuditoriaRepository logRepo, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _logRepo = logRepo;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest model)
        {
            // Busca o usuário ativo pelo e-mail
            var usuario = (await _usuarioService.ObterTodosAsync())
                .FirstOrDefault(u => u.Email == model.Email && u.Ativo);

            if (usuario == null)
                return Redirect("/Account/Login");

            // Mapeia para entidade para que PasswordHasher funcione corretamente
            var usuarioEntity = _mapper.Map<Usuario>(usuario);

            // Verifica a senha usando PasswordHasher
            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(usuarioEntity, usuarioEntity.SenhaHash, model.Senha);


            if (result == PasswordVerificationResult.Failed)
                return Redirect("/Account/Login");

            // Se a senha está válida, autentica o usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.PerfilUsuario.Nome)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.Now.AddDays(1)
                });

            // Registro de log
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = usuario.Nome,
                Entidade = nameof(Usuario),
                Acao = "Login",
                Descricao = $"Usuário {usuario.Nome} (ID: {usuario.Id}) entrou no sistema"
            });

            return Redirect("/");
        }




        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var nomeUsuario = HttpContext.User.Identity?.Name ?? "Desconhecido";

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = nomeUsuario,
                Entidade = nameof(Usuario),
                Acao = "Logout",
                Descricao = $"Usuário {nomeUsuario} saiu do sistema"
            });

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }
    }
}
