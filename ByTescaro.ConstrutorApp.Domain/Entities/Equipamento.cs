using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Equipamento
    {
        public long Id { get; set; }
        public bool Ativo { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Patrimonio { get; set; } = string.Empty;
        public StatusEquipamento Status { get; set; } = StatusEquipamento.Disponivel;
        public decimal CustoLocacaoDiaria { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

        // FK e Navegação
        public ICollection<ObraEquipamento> ProjetoEquipamentos { get; set; } = new List<ObraEquipamento>();

    }
}
