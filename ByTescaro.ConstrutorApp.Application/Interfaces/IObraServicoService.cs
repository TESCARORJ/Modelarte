using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraServicoService
    {
        Task<List<ObraServicoDto>> ObterPorListaIdAsync(long listaId);
        Task CriarAsync(ObraServicoDto dto);
        Task AtualizarAsync(ObraServicoDto dto);
        Task RemoverAsync(long id);
        Task<List<ServicoDto>> ObterServicosDisponiveisAsync(long obraId);
    }
}
