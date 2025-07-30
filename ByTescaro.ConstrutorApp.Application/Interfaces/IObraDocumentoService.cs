using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraDocumentoService
    {
        Task<List<ObraDocumentoDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraDocumentoDto dto);
        Task RemoverAsync(long id);
    }
}
