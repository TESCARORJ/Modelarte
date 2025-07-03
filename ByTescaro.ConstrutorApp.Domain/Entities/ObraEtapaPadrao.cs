using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEtapaPadrao : EntidadeBase
    {
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public StatusEtapa Status { get; set; }
        public ICollection<ObraItemEtapaPadrao> Itens { get; set; } = new List<ObraItemEtapaPadrao>();
    }

}
