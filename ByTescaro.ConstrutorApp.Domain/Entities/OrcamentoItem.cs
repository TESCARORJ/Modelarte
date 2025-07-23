using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class OrcamentoItem : EntidadeBase
    {
        public long OrcamentoObraId { get; set; }
        public long? InsumoId { get; set; }
        public long? ServicoId { get; set; }
        public long? FornecedorId { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        public OrcamentoObra OrcamentoObra { get; set; }
        public Insumo Insumo { get; set; } 
        public Servico Servico { get; set; } 
        public Fornecedor Fornecedor { get; set; } 
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
