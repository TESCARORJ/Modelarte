using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ParticipanteEvento : EntidadeBase
    {
        public long EventoId { get; set; } // FK para Evento
        public Evento? Evento { get; set; } // Navegação para o evento

        public long UsuarioId { get; set; } // FK para o Usuário convidado
        public Usuario? Usuario { get; set; } // Navegação para o usuário convidado
        public StatusParticipacao? StatusParticipacao { get; set; } // Ex: Pendente, Aceito, Recusado
        public DateTime DataResposta { get; set; }
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }
}
