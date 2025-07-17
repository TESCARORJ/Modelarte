using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEquipamento : EntidadeBase
    {
      
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = null!;

        public long EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; } = null!;

        public string? EquipamentoNome { get; set; } = string.Empty;

        public DateTime DataInicioUso { get; set; }
        public DateTime? DataFimUso { get; set; }
    }


}
