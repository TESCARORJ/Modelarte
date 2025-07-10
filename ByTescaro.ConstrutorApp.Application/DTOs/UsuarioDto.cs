namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class UsuarioDto
    {
        public long Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public string? Sobrenome { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? TelefonePrincipal { get; set; } = string.Empty;
        public string? TelefoneWhatsApp { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string? Senha { get; set; }
        public string? ConfirmarSenha { get; set; }
        public string? SenhaHash { get; set; }
        public long? PerfilUsuarioId { get; set; }
        public PerfilUsuarioDto? PerfilUsuario { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }

}
