namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraFornecedorDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long FornecedorId { get; set; }
        public string? FornecedorNome { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }

}
