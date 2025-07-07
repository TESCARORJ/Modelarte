using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IServicoService
{
    Task<IEnumerable<ServicoDto>> ObterTodosAsync();
    Task<ServicoDto?> ObterPorIdAsync(long id);
    Task CriarAsync(ServicoDto dto);
    Task AtualizarAsync(ServicoDto dto);
    Task RemoverAsync(long id);
    Task<bool> NomeExistsAsync(string nome, long? ignoreId = null);

}
