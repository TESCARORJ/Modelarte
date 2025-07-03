using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Fornecedor : Pessoa
    {

        public TipoFornecedor Tipo { get; set; }
    }

}
