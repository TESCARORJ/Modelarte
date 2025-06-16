namespace ByTescaro.ConstrutorApp.Domain.Entities
{
    public class ObraServico
    {
        public long Id { get; set; }
        public long ObraServicoListaId { get; set; }
        public long ServicoId { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public string UsuarioCadastro { get; set; } = string.Empty;

        // Navegação
        public ObraServicoLista Lista { get; set; } = null!;
        public Servico Servico { get; set; } = null!;
    }


}
