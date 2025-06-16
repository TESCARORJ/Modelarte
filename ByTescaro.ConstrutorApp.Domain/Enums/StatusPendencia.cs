using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum StatusPendencia
    {
        [Display(Name = "Pendente")]
        Pendente = 1,

        [Display(Name = "Em Execução")]
        EmExecucao = 2,

        [Display(Name = "Concluído")]
        Concluido = 3
    }
}
