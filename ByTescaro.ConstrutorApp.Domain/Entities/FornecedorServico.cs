using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class FornecedorServico : EntidadeBase
    {
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; } = null!;

        public long ServicoId { get; set; }
        public Servico Servico { get; set; } = null!;

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; }

        
    }

}
