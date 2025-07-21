using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum ResponsavelMaterialEnum
    {
        [Display(Name = "Cliente")]
        Cliente = 1,

        [Display(Name = "Empreiteita")]
        Empreiteira = 2
    }
}
