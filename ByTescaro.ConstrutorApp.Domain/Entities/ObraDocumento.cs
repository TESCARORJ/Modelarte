using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraDocumento : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = null!;
        public string? NomeOriginal { get; set; } = string.Empty;
        public string? CaminhoRelativo { get; set; } = string.Empty;
        public string? Extensao { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
    }
}
