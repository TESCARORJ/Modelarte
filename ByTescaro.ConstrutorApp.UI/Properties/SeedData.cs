//using ByTescaro.ConstrutorApp.Application.DTOs;
//using ByTescaro.ConstrutorApp.Application.Interfaces;

//namespace ByTescaro.ConstrutorApp.UI.Properties
//{
//    public static class DbSeeder
//    {
//        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
//        {
//            using var scope = serviceProvider.CreateScope();
//            var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();

//            var usuarios = await usuarioService.ObterTodosAsync();
//            var existe = usuarios.Any(u => u.Email == "administrador@bytescaro.com.br");       

//            if (!existe)
//            {
//                var usuario = new UsuarioDto
//                {
//                    Nome = "Administrador",
//                    Email = "administrador@bytescaro.com.br",
//                    Senha = "Admin@ByTescaro#001",
//                    TelefonePrincipal = "(00) 00000-0000",
//                    TelefoneWhatsApp = "(00) 00000-0000",
//                    Ativo = true,
//                    PerfilUsuarioId = 2,
//                    UsuarioCadastroId = 1,                    
//                    UsuarioCadastroNome = "sa",                    
//                    DataHoraCadastro = DateTime.Now
//                };

//                await usuarioService.CriarAsync(usuario);
//            }
//        }
//    }

//}
