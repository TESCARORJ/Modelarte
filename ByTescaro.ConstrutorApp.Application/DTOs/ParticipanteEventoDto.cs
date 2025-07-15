using ByTescaro.ConstrutorApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ParticipanteEventoDto
    {
        public long UsuarioId { get; set; }
        public string NomeUsuario { get; set; }
        public StatusParticipacao StatusParticipacao { get; set; }
    }
}
