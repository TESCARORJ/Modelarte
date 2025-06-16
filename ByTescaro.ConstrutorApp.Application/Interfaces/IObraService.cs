using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraService
    {
        Task<List<ObraDto>> ObterTodosAsync(); 
        Task<List<ObraDto>> ObterPorProjetoAsync(long projetoId);
        Task<ObraDto?> ObterPorIdAsync(long id);
        Task CriarAsync(ObraDto dto);
        Task AtualizarAsync(ObraDto dto);
        Task RemoverAsync(long id);

        Task<int> CalcularProgressoAsync(long obraId);
        Task<List<ObraEtapaDto>> ObterEtapasDaObraAsync(long obraId); 
        Task<List<ObraItemEtapaDto>> ObterItensDaEtapaAsync(long etapaId); 
        Task AtualizarConclusaoItemAsync(long itemId, bool concluido); 
    }
}
