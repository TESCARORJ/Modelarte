using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraImagem
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;
        public string NomeOriginal { get; set; } = string.Empty;
        public string CaminhoRelativo { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }

}
