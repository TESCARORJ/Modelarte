using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class OrcamentoItem : EntidadeBase
    {
        public long OrcamentoObraId { get; set; }
        public long? InsumoId { get; set; }
        public long? ServicoId { get; set; }
        public long? FornecedorId { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        public OrcamentoObra? OrcamentoObra { get; set; }
        public Insumo Insumo { get; set; } = null!;
        public Servico Servico { get; set; } = null!;
        public Fornecedor Fornecedor { get; set; } = null!;
    }
}
