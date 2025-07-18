using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraImagemDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string? NomeOriginal { get; set; } = string.Empty;
        public string? CaminhoRelativo { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public long UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
    }

}
