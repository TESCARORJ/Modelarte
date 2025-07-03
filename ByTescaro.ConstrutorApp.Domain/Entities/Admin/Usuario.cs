namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class Usuario : Pessoa
    {

        public string? SenhaHash { get; set; } = string.Empty;
        public string? Sobrenome { get; set; } = string.Empty;

        public long PerfilUsuarioId { get; set; }
        public PerfilUsuario PerfilUsuario { get; set; } = null!;
    }
}
