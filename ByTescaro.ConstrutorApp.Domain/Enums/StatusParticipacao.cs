using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusParticipacao
    {
        [Display(Name = "Pendente")]
        Pendente = 1, // Participação pendente de resposta

        [Display(Name = "Aceito")]
        Aceito = 2,    // Participação aceita pelo usuário

        [Display(Name = "Recusado")]
        Recusado = 3, // Participação recusada pelo usuário

        [Display(Name = "Cancelado")]
        Cancelado = 4 // Participação cancelada pelo criador do evento
    }
}
