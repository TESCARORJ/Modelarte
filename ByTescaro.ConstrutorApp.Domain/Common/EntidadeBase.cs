namespace ByTescaro.ConstrutorApp.Domain.Common
{
    public abstract class EntidadeBase
    {
        public long Id { get; set; }
        public bool? Ativo { get; set; }
        public DateTime? DataHoraCadastro { get; set; } = DateTime.Now;
        public long UsuarioCadastroId { get; set; }

    }
}
