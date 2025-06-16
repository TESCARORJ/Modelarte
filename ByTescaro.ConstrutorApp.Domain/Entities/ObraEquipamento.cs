namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEquipamento
    {
        public long Id { get; set; } 

        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public long EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; } = default!;

        public string EquipamentoNome { get; set; } = string.Empty;

        public DateTime DataInicioUso { get; set; }
        public DateTime? DataFimUso { get; set; }

        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
    }


}
