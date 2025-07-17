using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class LembreteEvento : EntidadeBase
    {
        public long EventoId { get; set; }
        public Evento Evento { get; set; } = null!;

        public int TempoAntes { get; set; } // Ex: 15, 30
        public UnidadeTempo? UnidadeTempo { get; set; } 
        public bool Enviado { get; set; } = false;
    }
}
