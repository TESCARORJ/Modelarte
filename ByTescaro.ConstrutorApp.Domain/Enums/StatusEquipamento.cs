using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums;

public enum StatusEquipamento
{
    [Display(Name = "Disponível")]
    Disponivel = 1,

    [Display(Name = "Em Uso")]
    EmUso = 2,

    [Display(Name = "Em Manutenção")]
    EmManutencao = 3,

    [Display(Name = "Indisponível")]
    Indisponivel = 4
}
