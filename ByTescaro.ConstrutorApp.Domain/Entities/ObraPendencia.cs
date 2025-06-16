using ByTescaro.ConstrutorApp.Domain.Enums;
using System;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraPendencia
    {
        public long Id { get; set; }

        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public StatusPendencia Status { get; set; } = StatusPendencia.Pendente;

        public long? ResponsavelId { get; set; }
        public Funcionario? Responsavel { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }

        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }
}
