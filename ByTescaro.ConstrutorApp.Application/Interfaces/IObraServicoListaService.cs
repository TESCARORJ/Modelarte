using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraServicoListaService
    {
        Task<List<ObraServicoListaDto>> ObterPorObraIdAsync(long obraId);
        Task<ObraServicoListaDto?> ObterPorIdAsync(long id);
        Task<ObraServicoListaDto> CriarAsync(ObraServicoListaDto dto);
        Task AtualizarAsync(ObraServicoListaDto dto);
        Task RemoverAsync(long id);

    }
}
