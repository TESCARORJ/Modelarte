using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFornecedorInsumoService
    {
        Task<List<FornecedorInsumoDto>> ObterTodosAsync();
        Task<FornecedorInsumoDto?> ObterPorIdAsync(long id);
        Task<List<FornecedorInsumoDto>> ObterPorFornecedorAsync(long fornecedorId);
        Task<List<FornecedorInsumoDto>> ObterPorInsumoAsync(long insumoId);
        Task<long> CriarAsync(FornecedorInsumoDto dto);
        Task AtualizarAsync(FornecedorInsumoDto dto);
        Task RemoverAsync(long id);
    }

}
