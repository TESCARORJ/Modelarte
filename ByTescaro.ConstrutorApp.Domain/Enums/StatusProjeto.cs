using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums;

public enum StatusProjeto
{
    Agendado = 1,

    [Display(Name = "Em Planejamento")]
    EmPlanejamento = 2,


    [Display(Name = "Em Andamento")]
    EmAndamento = 3,

    [Display(Name = "Concluído")]
    Concluido = 4,

    Cancelado = 5,

    Pausado = 6
}
