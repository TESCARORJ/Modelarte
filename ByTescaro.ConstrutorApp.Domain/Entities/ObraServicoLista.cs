using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class  ObraServicoLista
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long ResponsavelId { get; set; }
        public DateTime Data { get; set; }

        // Navegação
        public Obra Obra { get; set; } = null!;
        public Fornecedor Responsavel { get; set; } = null!;
        public ICollection<ObraServico> Itens { get; set; } = new List<ObraServico>();

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
