using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IProjetoService
    {
        Task<IEnumerable<ProjetoDto>> ObterTodosAsync();
        Task<ProjetoDto?> ObterPorIdAsync(long id);
        Task<ProjetoDto> CriarAsync(ProjetoDto dto);
        Task AtualizarAsync(ProjetoDto dto);
        Task RemoverAsync(long id);
        Task<IEnumerable<ProjetoDto>> ObterTodosAgendadosAsync();
        Task<IEnumerable<ProjetoDto>> ObterTodosEmPlanejamentoAsync();
        Task<IEnumerable<ProjetoDto>> ObterTodosEmAndamentoAsync();
        Task<IEnumerable<ProjetoDto>> ObterTodosConcluidosAsync();
        Task<IEnumerable<ProjetoDto>> ObterTodosCanceladosAsync();
        Task<IEnumerable<ProjetoDto>> ObterTodosPausadosAsync();
    }
}
