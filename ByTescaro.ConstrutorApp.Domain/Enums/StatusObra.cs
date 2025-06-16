using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusObra
    {
        [Display(Name = "Não Iniciada")]
        NaoIniciada = 1,

        [Display(Name = "Em Andamento")]
        EmAndamento = 2,

        [Display(Name = "Concluída")]
        Concluida = 3,

        [Display(Name = "Cancelada")]
        Cancelada = 4
    }
}

