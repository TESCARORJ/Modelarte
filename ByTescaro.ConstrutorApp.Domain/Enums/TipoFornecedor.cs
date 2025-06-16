using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum TipoFornecedor
    {
        Material = 1,

        [Display(Name = "Serviço")]
        Servico = 2,

        [Display(Name = "Meterial e Serviço")]
        Misto = 3
    }

}
