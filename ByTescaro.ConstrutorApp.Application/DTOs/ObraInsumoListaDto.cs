namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ObraInsumoListaDto
    {
        public long Id { get; set; }
        public long ObraId { get; set; }
        public long ResponsavelId { get; set; }
        public string? NomeResponsavel { get; set; } = string.Empty;
        public DateOnly Data { get; set; }
        public List<ObraInsumoDto> Itens { get; set; } = new();

        public DateTime DataHoraCadastro { get; set; }
        public string? UsuarioCadastro { get; set; } = string.Empty;
    }
}
