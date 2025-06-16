using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class FornecedorDto
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public TipoPessoa? TipoPessoa { get; set; }

        public string CpfCnpj { get; set; }
        public string TelefonePrincipal { get; set; } = string.Empty;
        public string TelefoneWhatsApp { get; set; } = string.Empty; public string Email { get; set; }
        public TipoFornecedor? Tipo { get; set; }

        public bool Ativo { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
