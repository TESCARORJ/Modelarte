using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IInsumoService
{
    Task<IEnumerable<InsumoDto>> ObterTodosAsync();
    Task<InsumoDto?> ObterPorIdAsync(long id);
    Task CriarAsync(InsumoDto dto);
    Task AtualizarAsync(InsumoDto dto);
    Task RemoverAsync(long id);
}
