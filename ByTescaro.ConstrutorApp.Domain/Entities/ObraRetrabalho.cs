using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraRetrabalho : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public StatusRetrabalho Status { get; set; } = StatusRetrabalho.Pendente;

        public long? ResponsavelId { get; set; }
        public Funcionario? Responsavel { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
    }
}
