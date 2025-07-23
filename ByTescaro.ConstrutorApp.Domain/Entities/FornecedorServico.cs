using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class FornecedorServico : EntidadeBase
    {  
        public long FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

        public long ServicoId { get; set; }
        public Servico Servico { get; set; } 

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;



    }

}
