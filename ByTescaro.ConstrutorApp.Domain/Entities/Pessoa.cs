using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public abstract class Pessoa : EntidadeBase
    {
        public string? Nome { get; set; } = string.Empty;
        public string? Sobrenome { get; set; } = string.Empty;
        public string? CpfCnpj { get; set; } = string.Empty;
        public TipoPessoa TipoPessoa { get; set; }
        public TipoEntidadePessoa TipoEntidade { get; set; }

        public string? TelefonePrincipal { get; set; } = string.Empty;
        public string? TelefoneWhatsApp { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public Endereco? Endereco { get; set; } = null!;
        public long? EnderecoId { get; set; }

 
    }
}
