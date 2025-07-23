using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraPendenciaDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string? Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;
        public StatusPendencia Status { get; set; }
        public long ResponsavelId { get; set; }
        public string? NomeResponsavel { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
    }
}
