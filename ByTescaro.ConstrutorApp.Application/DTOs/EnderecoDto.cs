using System.Text.Json.Serialization;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class EnderecoDto
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; } = string.Empty;

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; } = string.Empty;

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; } = string.Empty;

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; } = string.Empty;

        [JsonPropertyName("localidade")]
        public string? Cidade { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string? UF { get; set; } = string.Empty;

        [JsonPropertyName("ibge")]
        public string? Ibge { get; set; } = string.Empty;

        [JsonPropertyName("gia")]
        public string? Gia { get; set; } = string.Empty;

        [JsonPropertyName("ddd")]
        public string? Ddd { get; set; } = string.Empty;

        [JsonPropertyName("siafi")]
        public string? Siafi { get; set; } = string.Empty;

        // ⚠️ Esta propriedade só é retornada quando o CEP não existe
        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
