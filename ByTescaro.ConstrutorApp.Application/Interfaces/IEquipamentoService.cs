using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces;

public interface IEquipamentoService
{
    Task<IEnumerable<EquipamentoDto>> ObterTodosAsync();
    Task<EquipamentoDto?> ObterPorIdAsync(long id);
    Task CriarAsync(EquipamentoDto dto);
    Task AtualizarAsync(EquipamentoDto dto);
    Task RemoverAsync(long id);
    //Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync();

}
