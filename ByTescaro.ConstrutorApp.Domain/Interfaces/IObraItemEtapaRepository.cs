using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraItemEtapaRepository : IRepository<ObraItemEtapa>
    {
        Task<List<ObraItemEtapa>> GetByEtapaIdAsync(long obraEtapaId);
    }
}
