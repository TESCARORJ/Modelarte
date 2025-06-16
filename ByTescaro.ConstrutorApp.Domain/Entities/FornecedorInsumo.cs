namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class FornecedorInsumo
    {
        public long Id { get; set; }

        public long FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;

        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string Observacao { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }

}
