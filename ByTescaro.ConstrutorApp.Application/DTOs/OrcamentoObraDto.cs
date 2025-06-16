using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class OrcamentoObraDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public List<OrcamentoItemDto> Itens { get; set; } = new();
    }

}
