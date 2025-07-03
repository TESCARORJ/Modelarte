using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadrao : EntidadeBase
    {
        public long ObraEtapaPadraoId { get; set; }
        public ObraEtapaPadrao ObraEtapaPadrao { get; set; } = default!;

        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }
        public ICollection<ObraItemEtapaPadraoInsumo> Insumos { get; set; } = new List<ObraItemEtapaPadraoInsumo>();

    }

}
