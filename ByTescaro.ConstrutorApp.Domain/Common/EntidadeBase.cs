namespace ByTescaro.ConstrutorApp.Domain.Common
{
    public abstract class EntidadeBase
    {
        public long Id { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataHoraCadastro { get; set; } = DateTime.UtcNow;
        public string UsuarioCadastro { get; set; } = string.Empty;
    }
}
