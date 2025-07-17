using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;

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
    }
}
