using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraPendencia : EntidadeBase
    {

        public long ObraId { get; set; }
        public Obra Obra { get; set; } 

        public string? Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;

        public StatusPendencia Status { get; set; } = StatusPendencia.Pendente;

        public long? ResponsavelId { get; set; }
        public Funcionario? Responsavel { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
