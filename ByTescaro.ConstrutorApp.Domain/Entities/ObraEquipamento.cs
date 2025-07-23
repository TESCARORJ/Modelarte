using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEquipamento : EntidadeBase
    {
      
        public long ObraId { get; set; }
        public Obra Obra { get; set; }

        public long EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; }

        public string? EquipamentoNome { get; set; } = string.Empty;

        public DateTime DataInicioUso { get; set; }
        public DateTime? DataFimUso { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }


}
