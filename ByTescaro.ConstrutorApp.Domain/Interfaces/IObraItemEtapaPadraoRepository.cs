using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraItemEtapaPadraoRepository : IRepository<ObraItemEtapaPadrao>
    {
        Task<List<ObraItemEtapaPadrao>> GetByEtapaPadraoIdAsync(long obraEtapaId);

    }
}
