using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFornecedor : EntidadeBase
    {

        public long ObraId { get; set; }
        public Obra Obra { get; set; } = null!;

        public long FornecedorId { get; set; }
        public string? FornecedorNome { get; set; } = string.Empty;
        public Fornecedor Fornecedor { get; set; } = null!;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }


}
