using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Endereco : EntidadeBase
    {
        public string? Logradouro { get; set; } = string.Empty;
        public string? Numero { get; set; } = string.Empty;
        public string? Bairro { get; set; } = string.Empty;
        public string? Cidade { get; set; } = string.Empty;
        public string? Estado { get; set; } = string.Empty;
        public string? UF { get; set; } = string.Empty;
        public string? CEP { get; set; } = string.Empty;
        public string? Complemento { get; set; } = string.Empty;
    }
}
