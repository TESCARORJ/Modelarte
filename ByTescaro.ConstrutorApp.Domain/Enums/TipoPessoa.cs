using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums;

public enum TipoPessoa
{
    [Display(Name = "Física")]
    Fisica = 1,

    [Display(Name = "Jurídica")]
    Juridica = 2
}
