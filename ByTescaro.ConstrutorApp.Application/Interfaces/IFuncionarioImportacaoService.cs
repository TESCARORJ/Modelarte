using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFuncionarioImportacaoService
    {
        Task<List<FuncionarioDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarFuncionariosAsync(List<FuncionarioDto> funcionarios, string usuario);
    }

}
