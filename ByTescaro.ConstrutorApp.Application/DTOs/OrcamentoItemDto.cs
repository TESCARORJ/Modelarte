using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class OrcamentoItemDto
    {
        public long Id { get; set; }
        public long OrcamentoId { get; set; }
        public long? OrcamentoObraId { get; set; }
        public long? InsumoId { get; set; }
        public string? InsumoNome { get; set; } = string.Empty;
        public long? ServicoId { get; set; }
        public string? ServicoNome { get; set; } = string.Empty;
        public long? FornecedorId { get; set; }
        public string? FornecedorNome { get; set; } = string.Empty;

        public string? Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;


        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        public string? UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
