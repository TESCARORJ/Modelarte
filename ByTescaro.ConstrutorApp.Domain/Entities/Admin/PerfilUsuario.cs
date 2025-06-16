using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class PerfilUsuario
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } 
        public DateTime DataHoraCadastro { get; set; } = DateTime.UtcNow;
        public string UsuarioCadastro { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
