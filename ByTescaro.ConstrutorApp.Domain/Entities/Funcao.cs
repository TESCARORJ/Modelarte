using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Funcao : EntidadeBase
    {

        public string Nome { get; set; } = string.Empty; // Ex: Mestre de Obras, Engenheiro Civil
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;

    }
}
