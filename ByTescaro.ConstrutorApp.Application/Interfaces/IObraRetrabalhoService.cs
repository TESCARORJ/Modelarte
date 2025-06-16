using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraRetrabalhoService
    {
        Task<List<ObraRetrabalhoDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraRetrabalhoDto dto);
        Task AtualizarAsync(ObraRetrabalhoDto dto);
        Task RemoverAsync(long id);
    }
}
