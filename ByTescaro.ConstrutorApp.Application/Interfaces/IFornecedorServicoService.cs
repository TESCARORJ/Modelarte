using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFornecedorServicoService
    {
        Task<List<FornecedorServicoDto>> ObterTodosAsync();
        Task<FornecedorServicoDto?> ObterPorIdAsync(long id);
        Task<List<FornecedorServicoDto>> ObterPorFornecedorAsync(long fornecedorId);
        Task<List<FornecedorServicoDto>> ObterPorServicoAsync(long insumoId);
        Task<long> CriarAsync(FornecedorServicoDto dto);
        Task AtualizarAsync(FornecedorServicoDto dto);
        Task RemoverAsync(long id);
    }

}
