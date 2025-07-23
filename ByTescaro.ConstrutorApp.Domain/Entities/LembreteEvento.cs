using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class LembreteEvento : EntidadeBase
    {
        public long EventoId { get; set; }
        public Evento Evento { get; set; } 
        public int TempoAntes { get; set; } // Ex: 15, 30
        public UnidadeTempo? UnidadeTempo { get; set; } 
        public bool Enviado { get; set; } = false;
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
