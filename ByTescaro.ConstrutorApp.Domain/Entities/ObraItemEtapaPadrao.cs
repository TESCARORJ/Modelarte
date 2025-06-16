namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadrao
    {
        public long Id { get; set; }
        public long ObraEtapaPadraoId { get; set; }
        public ObraEtapaPadrao ObraEtapaPadrao { get; set; } = default!;

        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }

}
