using ByTescaro.ConstrutorApp.Domain.Common;

namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraDocumento : EntidadeBase
    {
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.Now;
        public string UsuarioCadastro { get; set; } = string.Empty;
        public long ObraId { get; set; }
        public Obra Obra { get; set; } = default!;

        public string NomeOriginal { get; set; } = string.Empty;
        public string CaminhoRelativo { get; set; } = string.Empty;
        public string Extensao { get; set; } = string.Empty;
        public long TamanhoEmKb { get; set; }
    }
}
