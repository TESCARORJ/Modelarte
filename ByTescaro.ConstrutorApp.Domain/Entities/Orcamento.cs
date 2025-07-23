using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Orcamento : EntidadeBase
    {
        public long ObraId { get; set; }
        public string? Responsavel { get; set; } = string.Empty;
        public DateTime DataReferencia { get; set; }
        public decimal TotalEstimado { get; set; }

        public Obra Obra { get; set; } 
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }

}
