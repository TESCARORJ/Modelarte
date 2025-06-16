using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IServicoImportacaoService
    {
        Task<List<ServicoDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarServicosAsync(List<ServicoDto> servicos, string usuario);
    }

}
