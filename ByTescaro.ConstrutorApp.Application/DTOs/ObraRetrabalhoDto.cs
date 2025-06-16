using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraRetrabalhoDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public StatusRetrabalho Status { get; set; }
        public long ResponsavelId { get; set; }
        public string NomeResponsavel { get; set; } = default!;
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
