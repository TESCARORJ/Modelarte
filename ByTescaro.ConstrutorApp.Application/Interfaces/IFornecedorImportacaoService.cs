using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IFornecedorImportacaoService
    {
        Task<List<FornecedorDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarFornecedoresAsync(List<FornecedorDto> fornecedores, string usuario);
    }

}
