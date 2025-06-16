using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IPerfilUsuarioService
{
    Task<IEnumerable<PerfilUsuarioDto>> ObterTodosAsync();
    Task<PerfilUsuarioDto?> ObterPorIdAsync(long id);
    Task CriarAsync(PerfilUsuarioDto dto);
    Task AtualizarAsync(PerfilUsuarioDto dto);
    Task RemoverAsync(long id);

}
