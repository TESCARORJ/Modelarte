using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraItemEtapaService
    {
        Task<List<ObraItemEtapaDto>> ObterTodasAsync();
        Task<List<ObraItemEtapaDto>> ObterPorEtapaIdAsync(long etapaId);
        Task AtualizarConclusaoAsync(long itemId, bool concluido);
        Task CriarAsync(ObraItemEtapaDto dto);
        Task AtualizarAsync(ObraItemEtapaDto dto);
        Task RemoverAsync(long id);
    }
}
