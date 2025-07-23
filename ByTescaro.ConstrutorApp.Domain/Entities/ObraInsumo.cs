using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraInsumo : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } 
        public long ObraInsumoListaId { get; set; }
        public long InsumoId { get; set; }
        public decimal Quantidade { get; set; }

        // Navegação
        public ObraInsumoLista Lista { get; set; } 
        public Insumo Insumo { get; set; } 
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }


}
