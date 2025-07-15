using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Evento : EntidadeBase
    {
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public bool IsRecorrente { get; set; } 
        public FrequenciaRecorrencia? FrequenciaRecorrencia { get; set; } // Ex: Diaria, Semanal, Mensal, Anual
        public DateTime? DataFimRecorrencia { get; set; }
        public long CriadorId { get; set; } // FK para o Usuário que criou o evento
        public Usuario? Criador { get; set; } // Navegação para o criador
        public Visibilidade? Visibilidade { get; set; } // Ex: Publico, Privado, SomenteConvidados
        public ICollection<ParticipanteEvento> Participantes { get; set; }
        public ICollection<LembreteEvento> Lembretes { get; set; } // Se houver lembretes específicos por evento
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }
}
