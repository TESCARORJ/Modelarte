using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IEquipamentoImportacaoService
    {
        Task<List<EquipamentoDto>> CarregarPreviewAsync(Stream excelStream);
        Task<List<ErroImportacaoDto>> ImportarEquipamentosAsync(List<EquipamentoDto> insumos, string usuario);
    }

}
