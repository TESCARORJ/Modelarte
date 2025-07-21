using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Enums
{
    public enum TipoEntidadePessoa
    {
        [Display(Name = "Usuário")]
        Usuario = 1,

        [Display(Name = "Cliente")]
        Cliente = 2,

        [Display(Name = "Funcionário")]
        Funcionario = 3,

        [Display(Name = "Fornecedor")]
        Fornecedor = 4
    }
}
