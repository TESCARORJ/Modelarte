namespace ByTescaro.ConstrutorApp.Application.DTOs.Relatorios
{
    public class ObraServicoListaRelatorioDto
    {
        public long Id { get; set; }
        public string? NomeResponsavel { get; set; }
        public DateOnly Data { get; set; }
        public List<ObraServicoRelatorioDto> Itens { get; set; } = new List<ObraServicoRelatorioDto>();
    }
}
