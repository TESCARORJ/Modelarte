using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraImagemService
    {
        Task<List<ObraImagemDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraImagemDto dto);
        Task RemoverAsync(long id);
    }
}
