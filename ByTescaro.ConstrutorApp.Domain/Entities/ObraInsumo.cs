namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraInsumo
    {
        public long Id { get; set; }
        public long ObraInsumoListaId { get; set; }
        public long InsumoId { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

        // Navegação
        public ObraInsumoLista Lista { get; set; } = null!;
        public Insumo Insumo { get; set; } = null!;
    }


}
