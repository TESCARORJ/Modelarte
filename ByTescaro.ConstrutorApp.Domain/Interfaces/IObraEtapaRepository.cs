using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraEtapaRepository : IRepository<ObraEtapa>
    {
        Task<List<ObraEtapa>> GetByObraIdAsync(long obraId);
        Task<ObraEtapa?> GetWithItensAsync(long etapaId);
    }
}
