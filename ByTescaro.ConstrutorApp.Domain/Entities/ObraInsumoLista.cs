using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraInsumoLista : EntidadeBase
    { 
        public long ObraId { get; set; }
        public long ResponsavelId { get; set; }
        public DateTime Data { get; set; }

        // Navegação
        public Obra Obra { get; set; } = null!;
        public Funcionario Responsavel { get; set; } = null!;
        public ICollection<ObraInsumo> Itens { get; set; } = new List<ObraInsumo>();
    }
}
