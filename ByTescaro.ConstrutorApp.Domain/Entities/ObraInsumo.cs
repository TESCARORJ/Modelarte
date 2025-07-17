using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraInsumo : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = null!;
        public long ObraInsumoListaId { get; set; }
        public long InsumoId { get; set; }
        public decimal Quantidade { get; set; }

        // Navegação
        public ObraInsumoLista Lista { get; set; } = null!;
        public Insumo Insumo { get; set; } = null!;
    }


}
