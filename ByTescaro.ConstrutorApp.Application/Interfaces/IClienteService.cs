using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteDto>> ObterTodosAsync();
    Task<ClienteDto?> ObterPorIdAsync(long id);
    Task CriarAsync(ClienteDto dto);
    Task AtualizarAsync(ClienteDto dto);
    Task RemoverAsync(long id);
}
