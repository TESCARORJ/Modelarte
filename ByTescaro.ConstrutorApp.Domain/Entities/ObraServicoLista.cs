using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraServicoLista : EntidadeBase
    {
        public long ObraId { get; set; }
        public long ResponsavelId { get; set; }
        public DateTime Data { get; set; }

        // Navegação
        public Obra Obra { get; set; } = null!;
        public Funcionario Responsavel { get; set; } = null!;
        public ICollection<ObraServico> Itens { get; set; } = new List<ObraServico>();
    }
}
