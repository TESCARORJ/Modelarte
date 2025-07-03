using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraServico : EntidadeBase
    {
        public long ObraServicoListaId { get; set; }
        public long ServicoId { get; set; }
        public decimal Quantidade { get; set; }

        // Navegação
        public ObraServicoLista Lista { get; set; } = null!;
        public Servico Servico { get; set; } = null!;
    }


}
