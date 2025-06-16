namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapa
    {
        public long Id { get; set; }
        public long ObraEtapaId { get; set; }
        public ObraEtapa ObraEtapa { get; set; } = default!;
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }

        public bool Concluido { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }
        public bool PrazoAtivo { get; set; }
        public DateTime? DataConclusao { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }

}
