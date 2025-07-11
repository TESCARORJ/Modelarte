using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class EventoDto
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public bool EhRecorrente { get; set; }
        public FrequenciaRecorrencia FrequenciaRecorrencia { get; set; }
        public DateTime? DataFimRecorrencia { get; set; }
        public long CriadorId { get; set; }
        public string NomeCriador { get; set; }
        public Visibilidade Visibilidade { get; set; }
        public List<ParticipanteEventoDto> Participantes { get; set; } = new List<ParticipanteEventoDto>();
        // Adicione outras propriedades se necessário
    }

   

    

    

    
}
