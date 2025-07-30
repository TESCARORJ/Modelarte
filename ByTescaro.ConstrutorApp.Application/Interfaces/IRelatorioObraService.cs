namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IRelatorioObraService
    {
        Task<byte[]> GerarRelatorioObraPdfAsync(long obraId);
    }
}
