using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraInsumoService
    {
        Task<List<ObraInsumoDto>> ObterPorListaIdAsync(long listaId);
        Task CriarAsync(ObraInsumoDto dto);
        Task AtualizarAsync(ObraInsumoDto dto);
        Task RemoverAsync(long id);
        Task<List<InsumoDto>> ObterInsumosDisponiveisAsync(long obraId);
    }
}
