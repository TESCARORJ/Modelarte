using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFornecedorService
    {
        Task<IEnumerable<FornecedorDto>> ObterTodosAsync();
        Task<FornecedorDto?> ObterPorIdAsync(long id);
        Task CriarAsync(FornecedorDto dto);
        Task AtualizarAsync(FornecedorDto dto);
        Task RemoverAsync(long id);
    }

}