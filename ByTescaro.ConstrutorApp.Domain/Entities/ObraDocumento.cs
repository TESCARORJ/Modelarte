using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraDocumento : EntidadeBase
    {
        public long ObraId { get; set; }
        public Obra Obra { get; set; } 
        public string? NomeOriginal { get; set; } = string.Empty;
        public string? CaminhoRelativo { get; set; } = string.Empty;
        public string? Extensao { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
        public long? UsuarioCadastroId { get; set; }
        public Usuario UsuarioCadastro { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
    }
}
