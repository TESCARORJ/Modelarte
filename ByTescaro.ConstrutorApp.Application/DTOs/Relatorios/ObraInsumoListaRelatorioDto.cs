namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraInsumoListaRelatorioDto
    {
        public long Id { get; set; }
        public string? NomeResponsavel { get; set; }
        public DateOnly Data { get; set; }
        public List<ObraInsumoRelatorioDto> Itens { get; set; } = new List<ObraInsumoRelatorioDto>();
    }
}
