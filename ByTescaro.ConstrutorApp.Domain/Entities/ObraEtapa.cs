using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEtapa : EntidadeBase
    {
    
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = null!;
        public string? Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public StatusEtapa Status { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }

        public ICollection<ObraItemEtapa> Itens { get; set; } = new List<ObraItemEtapa>();
    }

}
