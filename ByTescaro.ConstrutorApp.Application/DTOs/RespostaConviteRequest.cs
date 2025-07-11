using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class RespostaConviteRequest
    {
        public long EventoId { get; set; }
        public StatusParticipacao StatusParticipacao { get; set; } // "Accepted" ou "Declined"
    }
}
