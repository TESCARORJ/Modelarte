using System.ComponentModel.DataAnnotations;
namespace ByTescaro.ConstrutorApp.Domain.Enums
{

    public enum UnidadeMedida
    {
        [Display(Name = "Unidade(s)")]
        Unidade = 1,

        [Display(Name = "Metro(s)")]
        Metro = 2,

        [Display(Name = "Metro(s) Quadrado(s)")]
        MetroQuadrado = 3,

        [Display(Name = "Metro(s) Cúbico(s)")]
        MetroCubico = 4,

        [Display(Name = "Litro(s)")]
        Litro = 5,

        [Display(Name = "Quilo(s)")]
        Quilograma = 6,

        [Display(Name = "Tonelada(s)")]
        Tonelada = 7,

        [Display(Name = "Peça(s)")]
        Peca = 8,

        [Display(Name = "Caixa(s)")]
        Caixa = 9,

        [Display(Name = "Saco(s)")]
        Saco = 10,

        [Display(Name = "Rolo(s)")]
        Rolo = 11,

        [Display(Name = "Lata(s)")]
        Lata = 12,

        [Display(Name = "Tubo(s)")]
        Tubo = 13,

        [Display(Name = "Centímetro(s)")]
        Centimento = 14,

        [Display(Name = "Milímetro(s)")]
        Milimetro = 15
    }

}