using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusEtapa
    {
        [Display(Name = "Não Iniciado")]
        NaoIniciada = 1,

        [Display(Name = "Em Execução")]
        EmExecucao = 2,

        [Display(Name = "Concluída")]
        Concluida = 3,

        [Display(Name = "Reaberta")]
        Reaberta = 4
    }
}
