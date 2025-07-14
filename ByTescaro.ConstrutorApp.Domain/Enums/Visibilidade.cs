using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum Visibilidade
    {
        [Display(Name = "Público")]
        Publico = 1, // Evento visível para todos
       // Privado = 2, // Evento visível apenas para o criador
        
        [Display(Name = "Somente Convidados")]
        SomenteConvidados = 3 // Evento visível apenas para os convidados
    }
}
