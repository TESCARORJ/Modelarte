using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraItemEtapaPadraoInsumo : EntidadeBase
    {

        public long ObraItemEtapaPadraoId { get; set; }
        public ObraItemEtapaPadrao ObraItemEtapaPadrao { get; set; } 

        public long InsumoId { get; set; }
        public Insumo Insumo { get; set; } 

        public decimal QuantidadeSugerida { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
