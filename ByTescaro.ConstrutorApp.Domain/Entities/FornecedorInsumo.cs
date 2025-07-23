using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class FornecedorInsumo : EntidadeBase
    {       
        public long FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; }

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; } = string.Empty;
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;


    }

}
