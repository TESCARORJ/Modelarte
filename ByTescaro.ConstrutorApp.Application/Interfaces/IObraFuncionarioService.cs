using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraFuncionarioService
    {
        Task<List<ObraFuncionarioDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraFuncionarioDto dto);
        Task AtualizarAsync(ObraFuncionarioDto dto);
        Task RemoverAsync(long id);
        Task<List<FuncionarioDto>> ObterFuncionariosDisponiveisAsync(long obraId);
        Task<List<FuncionarioDto>> ObterFuncionariosTotalDisponiveisAsync();
        Task<List<FuncionarioDto>> ObterFuncionariosTotalAlocadosAsync();

    }
}
