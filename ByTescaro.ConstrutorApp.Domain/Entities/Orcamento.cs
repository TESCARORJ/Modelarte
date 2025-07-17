using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Orcamento : EntidadeBase
    {
        public long ObraId { get; set; }
        public string? Responsavel { get; set; } = string.Empty;
        public DateTime DataReferencia { get; set; }
        public decimal TotalEstimado { get; set; }

        public Obra Obra { get; set; } = null!;
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
    }

}
