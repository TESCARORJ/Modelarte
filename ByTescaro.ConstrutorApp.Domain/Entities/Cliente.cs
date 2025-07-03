using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Cliente : Pessoa
    {

        public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
    }
}
