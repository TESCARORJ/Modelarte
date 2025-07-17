using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraEquipamentoDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long EquipamentoId { get; set; }
        public string? EquipamentoNome { get; set; } = string.Empty;

        public DateTime DataInicioUso { get; set; }
        public DateTime? DataFimUso { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }

}
