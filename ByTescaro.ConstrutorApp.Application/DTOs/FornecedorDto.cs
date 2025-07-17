using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class FornecedorDto
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public TipoPessoa? TipoPessoa { get; set; }

        public string? CpfCnpj { get; set; }
        public string? TelefonePrincipal { get; set; } = string.Empty;
        public string? TelefoneWhatsApp { get; set; } = string.Empty; 
        public string? Email { get; set; }
        public TipoFornecedor? Tipo { get; set; }

        public bool Ativo { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

        /// <summary>
        /// Cria uma cópia superficial (shallow copy) da instância atual do FornecedorDto.
        /// Este método é útil para preservar o estado original do DTO em operações de edição.
        /// </summary>
        /// <returns>Uma nova instância de FornecedorDto com os mesmos valores de propriedade.</returns>
        public FornecedorDto Clone()
        {
            // MemberwiseClone é um método protegido, só pode ser chamado de dentro da própria classe ou derivado.
            // O tipo de retorno é object, então precisamos de um cast.
            return (FornecedorDto)this.MemberwiseClone();
        }
    }

}
