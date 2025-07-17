using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraEtapaPadraoService
    {
        Task<List<ObraEtapaPadraoDto>> ObterTodasAsync();
        Task<ObraEtapaPadraoDto?> ObterPorIdAsync(long id);
        Task<List<ObraEtapaPadraoDto>> ObterPorObraIdAsync(long obraId);
        Task<ObraEtapaPadraoDto?> ObterComItensAsync(long etapaId);
        //Task AtualizarStatusAsync(long etapaId, StatusEtapa novoStatus);
        Task CriarAsync(ObraEtapaPadraoDto dto);
        Task AtualizarAsync(ObraEtapaPadraoDto dto);
        Task RemoverAsync(long id);
    }
}
