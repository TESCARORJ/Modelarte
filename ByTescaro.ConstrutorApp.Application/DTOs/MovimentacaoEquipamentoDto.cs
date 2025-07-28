namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class MovimentacaoEquipamentoDto
    {
        public long EquipamentoId { get; set; }
        public long ObraOrigemId { get; set; }
        public long ObraDestinoId { get; set; }
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;
        public string? Motivo { get; set; }
    }
}
