using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusOrcamento
    {
        [Display(Name = "Planejado")]
        Planejado = 1,

        [Display(Name = "Revisado")]
        Revisado = 2,

        [Display(Name = "Aprovado")]
        Aprovado = 3,

        [Display(Name = "Executado")]
        Executado = 4,

        [Display(Name = "Cancelado")]
        Cancelado = 5
    }
}
