using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class Usuario 
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataHoraCadastro { get; set; } = DateTime.UtcNow;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long PerfilUsuarioId { get; set; }
        public PerfilUsuario PerfilUsuario { get; set; } = null!;
    }
}
