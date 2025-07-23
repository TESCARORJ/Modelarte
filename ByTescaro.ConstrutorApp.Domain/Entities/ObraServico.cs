using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraServico : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } 
        public long ObraServicoListaId { get; set; }
        public long ServicoId { get; set; }
        public decimal Quantidade { get; set; }

        // Navegação
        public ObraServicoLista Lista { get; set; } 
        public Servico Servico { get; set; } 
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }


}
