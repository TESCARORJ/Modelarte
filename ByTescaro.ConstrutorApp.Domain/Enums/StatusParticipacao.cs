using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusParticipacao
    {
        Pendente = 1, // Participação pendente de resposta
        Aceito = 2,   // Participação aceita pelo usuário
        Recusado = 3, // Participação recusada pelo usuário
        Cancelado = 4 // Participação cancelada pelo criador do evento
    }
}
