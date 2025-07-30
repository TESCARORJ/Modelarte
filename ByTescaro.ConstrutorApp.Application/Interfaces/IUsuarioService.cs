using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> ObterTodosAsync();
        Task<IEnumerable<UsuarioDto>> ObterTodosAtivosAsync();
        Task<UsuarioDto?> ObterPorIdAsync(long id);
        Task CriarAsync(UsuarioDto dto);
        Task AtualizarAsync(UsuarioDto dto);
        Task InativarAsync(long id, string atualizadoPor);
        Task ExcluirAsync(long id);

        //Task<PerfilUsuario?> ObterPerfilUsuarioAsync(long id);
        //Task<IEnumerable<PerfilUsuario>> ObterTodosPerfisAsync();
    }
}
