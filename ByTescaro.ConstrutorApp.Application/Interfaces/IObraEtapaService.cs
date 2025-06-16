using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Enums;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraEtapaService
    {
        Task<List<ObraEtapaDto>> ObterTodasAsync();

        Task<List<ObraEtapaDto>> ObterPorObraIdAsync(long obraId);
        Task<ObraEtapaDto?> ObterComItensAsync(long etapaId);
        Task AtualizarStatusAsync(long etapaId, StatusEtapa novoStatus);
        Task CriarAsync(ObraEtapaDto dto);
        Task AtualizarAsync(ObraEtapaDto dto);
        Task RemoverAsync(long id);
    }
}
