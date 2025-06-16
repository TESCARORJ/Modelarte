namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFornecedor
    {
        public long Id { get; set; } 

        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public long FornecedorId { get; set; }
        public string FornecedorNome { get; set; } = string.Empty;
        public Fornecedor Fornecedor { get; set; } = default!;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }


}
