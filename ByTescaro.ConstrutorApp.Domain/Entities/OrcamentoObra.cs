using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class OrcamentoObra : EntidadeBase
    {
        public long ObraId { get; set; }
        public DateTime DataReferencia { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public StatusOrcamento Status { get; set; }
        public decimal TotalEstimado { get; set; }
        public Obra? Obra { get; set; }
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
