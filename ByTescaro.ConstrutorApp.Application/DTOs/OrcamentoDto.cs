using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class OrcamentoDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public string? Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Total { get; set; }
        public long UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
        public bool Ativo { get; set; }
        public List<OrcamentoItemDto> Itens { get; set; } = new();
        public DateTime? DataReferencia { get; set; }
        public string? Responsavel { get; set; }
    }
}
