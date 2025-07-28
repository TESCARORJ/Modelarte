using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraInsumoDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long ObraInsumoListaId { get; set; }
        public long InsumoId { get; set; }
        public string? InsumoNome { get; set; } = string.Empty;
        public UnidadeMedida? UnidadeMedida { get; set; }
        public bool IsRecebido { get; set; } = false;
        public DateTime? DataRecebimento { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
    }

}
