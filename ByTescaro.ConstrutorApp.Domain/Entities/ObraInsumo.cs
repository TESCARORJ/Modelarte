using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraInsumo : EntidadeBase
    {
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long ObraInsumoListaId { get; set; }
        public long InsumoId { get; set; }
        public decimal Quantidade { get; set; }

        // Navegação
        public ObraInsumoLista Lista { get; set; } = null!;
        public Insumo Insumo { get; set; } = null!;
    }


}
