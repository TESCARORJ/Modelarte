using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Fornecedor
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public TipoPessoa? TipoPessoa { get; set; }
        public string CpfCnpj { get; set; } = string.Empty;
        public string TelefonePrincipal { get; set; } = string.Empty;
        public string TelefoneWhatsApp { get; set; } = string.Empty; 
        public string Email { get; set; }
        public TipoFornecedor Tipo { get; set; }
        public bool Ativo { get; set; } = true;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
