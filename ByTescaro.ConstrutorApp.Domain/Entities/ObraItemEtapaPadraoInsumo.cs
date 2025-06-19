namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadraoInsumo
    {
        public long Id { get; set; }

        public long ObraItemEtapaPadraoId { get; set; }
        public ObraItemEtapaPadrao ObraItemEtapaPadrao { get; set; } = default!;

        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; } = default!;

        public decimal QuantidadeSugerida { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
