using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IInsumoImportacaoService
    {
        Task<List<InsumoDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarInsumosAsync(List<InsumoDto> insumos, string usuario);
    }

}
