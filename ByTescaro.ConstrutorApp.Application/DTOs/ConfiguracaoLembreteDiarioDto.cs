using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ConfiguracaoLembreteDiarioDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "A hora do dia é obrigatória.")]
        [DataType(DataType.Time)]
        [Display(Name = "Hora do Dia")]
        public TimeSpan HoraDoDia { get; set; }

        [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; } = string.Empty;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; }
    }
}
