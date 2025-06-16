using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraEtapaPadrao
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
        public StatusEtapa Status { get; set; }

        public ICollection<ObraItemEtapaPadrao> Itens { get; set; } = new List<ObraItemEtapaPadrao>();
    }

}
