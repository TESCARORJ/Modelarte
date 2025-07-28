using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraEquipamentoService
    {
        Task<List<ObraEquipamentoDto>> ObterPorObraIdAsync(long obraId);
        Task CriarAsync(ObraEquipamentoDto dto);
        Task AtualizarAsync(ObraEquipamentoDto dto);
        Task RemoverAsync(long id);
        Task<List<EquipamentoDto>> ObterEquipamentosDisponiveisAsync(long obraId);
        Task<List<EquipamentoDto>> ObterEquipamentosTotalDisponiveisAsync();
        Task<List<EquipamentoDto>> ObterEquipamentosTotalAlocadosAsync();
        Task MoverEquipamentoAsync(MovimentacaoEquipamentoDto dto);

    }
}
