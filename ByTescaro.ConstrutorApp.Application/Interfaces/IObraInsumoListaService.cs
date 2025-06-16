using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraInsumoListaService
    {
        Task<List<ObraInsumoListaDto>> ObterPorObraIdAsync(long obraId);
        Task<ObraInsumoListaDto?> ObterPorIdAsync(long id);
        Task<ObraInsumoListaDto> CriarAsync(ObraInsumoListaDto dto);
        Task AtualizarAsync(ObraInsumoListaDto dto);
        Task RemoverAsync(long id);
    }
}
