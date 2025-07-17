using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Equipamento : EntidadeBase
    {
        public string? Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; } = string.Empty;
        public string? Patrimonio { get; set; } = string.Empty;
        public StatusEquipamento Status { get; set; } = StatusEquipamento.Disponivel;
        public decimal CustoLocacaoDiaria { get; set; }

        // FK e Navegação
        public ICollection<ObraEquipamento> ProjetoEquipamentos { get; set; } = new List<ObraEquipamento>();

    }
}
