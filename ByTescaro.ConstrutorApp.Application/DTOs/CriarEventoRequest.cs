using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class CriarEventoRequest
    {
        public string? Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; } = DateTime.Now; 
        public DateTime DataHoraFim { get; set; } = DateTime.Now.AddHours(1); 
        public bool EhRecorrente { get; set; }
        public FrequenciaRecorrencia FrequenciaRecorrencia { get; set; }
        public DateTime? DataFimRecorrencia { get; set; }
        public Visibilidade Visibilidade { get; set; }
        public List<long> IdsParticipantesConvidados { get; set; } = new List<long>();
    }
}
