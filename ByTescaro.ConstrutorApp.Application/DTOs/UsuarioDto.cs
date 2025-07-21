namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class UsuarioDto
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public string? Email { get; set; }
        public string? TelefonePrincipal { get; set; }
        public string? TelefoneWhatsApp { get; set; }
        public bool Ativo { get; set; }
        public string? Senha { get; set; }
        public string? ConfirmarSenha { get; set; }
        public string? SenhaHash { get; set; }
        public long? PerfilUsuarioId { get; set; }
        public PerfilUsuarioDto? PerfilUsuario { get; set; }

        // Endereço
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? UF { get; set; }
        public string? CEP { get; set; }
        public string? Complemento { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; }
    }

}
