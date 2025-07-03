using ByTescaro.ConstrutorApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraImagem : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;
        public string NomeOriginal { get; set; } = string.Empty;
        public string CaminhoRelativo { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
    }

}
