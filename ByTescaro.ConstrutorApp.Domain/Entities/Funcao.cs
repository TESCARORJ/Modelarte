namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class Funcao
    {
        public long Id { get; set; }

        public string Nome { get; set; } = string.Empty; // Ex: Mestre de Obras, Engenheiro Civil

        public bool Ativo { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }

    }
}
