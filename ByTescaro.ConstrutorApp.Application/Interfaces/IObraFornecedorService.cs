using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraFornecedorService
    {
        Task<List<ObraFornecedorDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraFornecedorDto dto);
        Task AtualizarAsync(ObraFornecedorDto dto);
        Task RemoverAsync(long id);
        Task<List<FornecedorDto>> ObterFornecedoresDisponiveisAsync(long obraId);
        Task<List<FornecedorDto>> ObterFornecedoresTotalDisponiveisAsync();
        Task<List<FornecedorDto>> ObterFornecedoresTotalAlocadosAsync();

    }
}
