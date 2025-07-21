using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum FrequenciaRecorrencia
    {
        [Display(Name = "Diária")]
        Diaria = 1,

        [Display(Name = "Semanal")]
        Semanal = 2,

        [Display(Name = "Mensal")]
        Mensal = 3,

        [Display(Name = "Anual")]
        Anual = 4,
    }
}
