using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraDocumentoRepository : IRepository<ObraDocumento>
    {
        Task<List<ObraDocumento>> GetByObraIdAsync(long obraId);
    }
}
