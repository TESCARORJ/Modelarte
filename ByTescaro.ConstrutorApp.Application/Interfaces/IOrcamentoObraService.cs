using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IOrcamentoObraService
    {
        Task<List<OrcamentoObraDto>> ObterPorObraIdAsync(long obraId);
    }

}
