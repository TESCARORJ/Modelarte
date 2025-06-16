using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEtapa
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public StatusEtapa Status { get; set; }

        public DateTime? DataInicio { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

        public ICollection<ObraItemEtapa> Itens { get; set; } = new List<ObraItemEtapa>();
    }

}
