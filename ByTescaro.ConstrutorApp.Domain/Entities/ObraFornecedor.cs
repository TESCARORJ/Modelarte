using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraFornecedor : EntidadeBase
    {

        public long ObraId { get; set; }
        public Obra Obra { get; set; } 

        public long FornecedorId { get; set; }
        public string? FornecedorNome { get; set; } = string.Empty;
        public Fornecedor Fornecedor { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }


}
