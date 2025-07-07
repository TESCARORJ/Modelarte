using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFornecedor : EntidadeBase
    {
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public long FornecedorId { get; set; }
        public string FornecedorNome { get; set; } = string.Empty;
        public Fornecedor Fornecedor { get; set; } = default!;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }


}
