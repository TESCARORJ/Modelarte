using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Entities.Admin
{
    public class PerfilUsuario : EntidadeBase
    {
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
