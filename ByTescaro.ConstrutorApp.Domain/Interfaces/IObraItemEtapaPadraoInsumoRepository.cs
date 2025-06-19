using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraItemEtapaPadraoInsumoRepository : IRepository<ObraItemEtapaPadraoInsumo>
    {
        Task<List<ObraItemEtapaPadraoInsumo>> GetByItemPadraoIdAsync(long itemPadraoId);
    }
}
