using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IFuncaoService
{
    Task<IEnumerable<FuncaoDto>> ObterTodasAsync();
    Task<FuncaoDto?> ObterPorIdAsync(long id);
    Task<FuncaoDto?> ObterPorNomeAsync(string nome);
    Task CriarAsync(FuncaoDto dto);
    Task AtualizarAsync(FuncaoDto dto);
    Task RemoverAsync(long id);
    Task<bool> NomeExistsAsync(string nome, long? ignoreId = null);

}
