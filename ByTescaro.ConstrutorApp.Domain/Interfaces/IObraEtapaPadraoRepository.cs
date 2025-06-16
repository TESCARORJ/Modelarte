using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraEtapaPadraoRepository : IRepository<ObraEtapaPadrao>
    {
        Task<List<ObraEtapaPadrao>> GetByObraIdAsync(long obraId);
        Task<ObraEtapaPadrao?> GetWithItensAsync(long etapaId);
    }
}
