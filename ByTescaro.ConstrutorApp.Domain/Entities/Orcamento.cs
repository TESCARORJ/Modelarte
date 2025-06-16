using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Orcamento
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public DateTime DataReferencia { get; set; }
        public decimal TotalEstimado { get; set; }

        public Obra Obra { get; set; } = default!;
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();

        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
