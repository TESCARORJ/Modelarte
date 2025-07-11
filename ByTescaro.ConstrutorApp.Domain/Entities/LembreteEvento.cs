using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class LembreteEvento : EntidadeBase
    {
        public long EventoId { get; set; }
        public Evento Evento { get; set; }

        public int TempoAntes { get; set; } // Ex: 15, 30
        public UnidadeTempo? UnidadeTempo { get; set; } // Ex: Minutos, Horas, Dias
        public bool Enviado { get; set; } = false; // Para controlar se o lembrete já foi enviado
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }
}
