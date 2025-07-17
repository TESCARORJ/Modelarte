using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadraoInsumo : EntidadeBase
    {

        public long ObraItemEtapaPadraoId { get; set; }
        public ObraItemEtapaPadrao ObraItemEtapaPadrao { get; set; } = default!;

        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; } = default!;

        public decimal QuantidadeSugerida { get; set; }
    }
}
