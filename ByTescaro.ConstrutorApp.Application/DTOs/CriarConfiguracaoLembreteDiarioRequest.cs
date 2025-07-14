using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class CriarConfiguracaoLembreteDiarioRequest
    {
        [Required(ErrorMessage = "A hora do dia é obrigatória.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora do Dia")]
        public TimeSpan HoraDoDia { get; set; }

        [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }

}
