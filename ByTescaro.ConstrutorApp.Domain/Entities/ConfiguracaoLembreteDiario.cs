using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using System.ComponentModel.DataAnnotations;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ConfiguracaoLembreteDiario : EntidadeBase
    {
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Hora do Dia")]
        public TimeSpan HoraDoDia { get; set; } // Armazena apenas a hora (ex: 12:00:00)

        [MaxLength(100)]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
