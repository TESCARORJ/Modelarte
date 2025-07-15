using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public long UsuarioCadastroId { get; set; }
        public Usuario? UsuarioCadastro { get; set; }

        // Opcional: Se cada usuário puder ter suas próprias configurações de lembrete diário
        // public long UsuarioId { get; set; }
        // public Usuario Usuario { get; set; }
    }
}
