using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    namespace ByTescaro.ConstrutorApp.Domain.Interfaces
    {
        public interface IObraPendenciaRepository : IRepository<ObraPendencia>
        {
            Task<List<ObraPendencia>> GetByObraIdAsync(long obraId);
        }
    }
}
