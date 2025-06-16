using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum TipoMovimentacao
    {
        Entrada = 1,

        [Display(Name = "Saída")]
        Saida = 2,

        //Ajuste = 3
    }

}
