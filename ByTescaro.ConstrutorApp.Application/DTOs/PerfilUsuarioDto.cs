using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class PerfilUsuarioDto
    {
        public long Id { get; set; } 
        public string? Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
