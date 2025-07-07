//using ByTescaro.ConstrutorApp.Application.Interfaces;
//using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//public class AuthService
//{
//    private readonly IConfiguration _config;
//    private readonly IUsuarioService _usuarioService;

//    public AuthService(IConfiguration config, IUsuarioService usuarioService)
//    {
//        _config = config;
//        _usuarioService = usuarioService;
//    }

//    public async Task<string?> LoginAsync(string email, string senha)
//    {
//        var usuario = (await _usuarioService.ListarAsync())
//            .FirstOrDefault(u => u.Email == email && u.SenhaHash == senha && u.Ativo);

//        if (usuario == null) return null;

//        return GerarToken(usuario);
//    }

//    public string GerarToken(Usuario usuario)
//    {
//        var claims = new[]
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
//            new Claim(ClaimTypes.Name, usuario.Nome),
//            new Claim(ClaimTypes.Email, usuario.Email),
//            new Claim(ClaimTypes.Role, usuario.PerfilUsuario?.Nome ?? "Usuario"),
//            new Claim("usuarioId", usuario.Id.ToString())
//        };

//        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

//        var token = new JwtSecurityToken(
//            issuer: _config["Jwt:Issuer"],
//            audience: _config["Jwt:Audience"],
//            claims: claims,
//            expires: DateTime.Now.AddHours(1),
//            signingCredentials: credenciais
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}
