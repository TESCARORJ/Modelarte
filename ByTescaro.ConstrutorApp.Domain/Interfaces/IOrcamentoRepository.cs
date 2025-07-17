using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IOrcamentoRepository : IRepository<Orcamento>
    {
        Task<List<Orcamento>> GetByObraAsync(long obraId);
        Task<Orcamento?> GetByIdComItensAsync(long id);
        Task<Orcamento?> GetByIdComItensNoTrackingAsync(long id);

    }
}
