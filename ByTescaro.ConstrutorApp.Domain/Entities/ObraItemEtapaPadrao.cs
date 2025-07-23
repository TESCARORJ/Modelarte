using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadrao : EntidadeBase
    {
        public long ObraEtapaPadraoId { get; set; }
        public ObraEtapaPadrao ObraEtapaPadrao { get; set; }

        public string? Nome { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public bool IsDataPrazo { get; set; }
        public int? DiasPrazo { get; set; }
        public ICollection<ObraItemEtapaPadraoInsumo> Insumos { get; set; } = new List<ObraItemEtapaPadraoInsumo>();
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;

    }

}
