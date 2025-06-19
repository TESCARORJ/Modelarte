using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraItemEtapaPadraoInsumoService
    {
        Task<List<ObraItemEtapaPadraoInsumoDto>> ObterPorItemPadraoIdAsync(long itemPadraoId);
        Task CriarAsync(ObraItemEtapaPadraoInsumoDto dto);
        Task AtualizarAsync(ObraItemEtapaPadraoInsumoDto dto);
        Task RemoverAsync(long id);
    }
}
