using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraPendenciaService
    {
        Task<List<ObraPendenciaDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraPendenciaDto dto);
        Task AtualizarAsync(ObraPendenciaDto dto);
        Task RemoverAsync(long id);
    }
}
