using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IFuncionarioService
{
    Task<IEnumerable<FuncionarioDto>> ObterTodosAsync();
    Task<FuncionarioDto?> ObterPorIdAsync(long id);
    Task CriarAsync(FuncionarioDto dto);
    Task AtualizarAsync(FuncionarioDto dto);
    Task RemoverAsync(long id);

    //Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync();
}
