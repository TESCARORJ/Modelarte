namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class OrcamentoItem
    {
        public long Id { get; set; }
        public long OrcamentoObraId { get; set; }
        public long? InsumoId { get; set; }
        public long? ServicoId { get; set; }
        public long? FornecedorId { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        public OrcamentoObra? OrcamentoObra { get; set; }
        public Insumo Insumo { get; set; } = default!;
        public Servico Servico { get; set; } = default!;
        public Fornecedor Fornecedor { get; set; } = default!;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }
}
