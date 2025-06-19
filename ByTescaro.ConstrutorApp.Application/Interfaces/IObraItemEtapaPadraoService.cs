using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraItemEtapaPadraoService
    {
        Task<ObraItemEtapaPadraoDto?> ObterPorIdAsync(long id);
        Task<List<ObraItemEtapaPadraoDto>> ObterTodasAsync();
        Task<List<ObraItemEtapaPadraoDto>> ObterPorEtapaIdAsync(long etapaId);
        Task AtualizarConclusaoAsync(long itemId, bool concluido);
        Task<ObraItemEtapaPadraoDto> CriarAsync(ObraItemEtapaPadraoDto dto);
        Task AtualizarAsync(ObraItemEtapaPadraoDto dto);
        Task RemoverAsync(long id);
    }
}
