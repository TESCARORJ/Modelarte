namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class Usuario : Pessoa
    {

        public string? SenhaHash { get; set; } = string.Empty;
        public string? Sobrenome { get; set; } = string.Empty;

        public long PerfilUsuarioId { get; set; }
        public PerfilUsuario PerfilUsuario { get; set; } = null!;
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
