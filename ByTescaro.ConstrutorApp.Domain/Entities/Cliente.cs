using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Cliente
    {
        public long Id { get; set; }
        public bool Ativo { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public TipoPessoa TipoPessoa { get; set; }

        // Endereço
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;

        // Contato
        public string TelefonePrincipal { get; set; } = string.Empty;
        public string TelefoneWhatsApp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Auditoria
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;


        // Navegação
        public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
    }
}
