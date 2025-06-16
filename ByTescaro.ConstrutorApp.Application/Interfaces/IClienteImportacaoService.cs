using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IClienteImportacaoService
    {
        Task<List<ClienteDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarClientesAsync(List<ClienteDto> clientes, string usuario);
    }

}
