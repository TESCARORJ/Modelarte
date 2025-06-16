namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class FornecedorInsumoDto
    {
        public long Id { get; set; }

        public long FornecedorId { get; set; }
        public string FornecedorNome { get; set; } = string.Empty;

        public long InsumoId { get; set; }
        public string InsumoNome { get; set; } = string.Empty;

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public bool Ativo { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

    }

}
