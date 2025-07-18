namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class FornecedorServicoDto
    {
        public long Id { get; set; }

        public long FornecedorId { get; set; }
        public string? FornecedorNome { get; set; }

        public long ServicoId { get; set; }
        public string? ServicoNome { get; set; }

        public decimal PrecoUnitario { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; }

        public bool Ativo { get; set; }
        public long UsuarioCadastroId { get; set; }
        public string? UsuarioCadastroNome { get; set; } = string.Empty;
        public DateTime DataHoraCadastro { get; set; }
    }

}
