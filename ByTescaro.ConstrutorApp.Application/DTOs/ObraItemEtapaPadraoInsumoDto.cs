using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraItemEtapaPadraoInsumoDto
    {
        public long Id { get; set; }
        public long ObraItemEtapaPadraoId { get; set; }
        public long InsumoId { get; set; }
        public string InsumoNome { get; set; } = string.Empty;
        public UnidadeMedida? UnidadeMedida { get; set; }

        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
