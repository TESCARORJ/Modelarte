namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class MovimentacaoDialogResult
    {
        public long ObraDestinoId { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string? Motivo { get; set; }
        public bool Confirmed { get; set; } // Para indicar se o usuário confirmou ou cancelou
    }
}