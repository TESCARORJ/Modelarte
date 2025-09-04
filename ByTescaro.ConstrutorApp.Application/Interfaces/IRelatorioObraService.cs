using ByTescaro.ConstrutorApp.Application.DTOs.Relatorios;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IRelatorioObraService
    {
        Task<ObraRelatorioDto?> GetRelatorioAsync(long obraId, CancellationToken ct = default);
        Task<byte[]> GerarRelatorioObraPdfAsync(long obraId);
    }
}
