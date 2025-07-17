using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;

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
        public long UsuarioCadastroId { get; set; } // FK para o Usuário que criou o evento
        public Usuario? Criador { get; set; } = null!;
        public Visibilidade? Visibilidade { get; set; } // Ex: Publico, Privado, SomenteConvidados
        public ICollection<ParticipanteEvento> Participantes { get; set; }
        public ICollection<LembreteEvento> Lembretes { get; set; } // Se houver lembretes específicos por evento
    }
}
