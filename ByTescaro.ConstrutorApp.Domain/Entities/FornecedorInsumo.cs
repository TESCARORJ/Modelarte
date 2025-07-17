using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class FornecedorInsumo : EntidadeBase
    {       
        public long FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;
        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; } = null!;

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; } = string.Empty;

      
    }

}
