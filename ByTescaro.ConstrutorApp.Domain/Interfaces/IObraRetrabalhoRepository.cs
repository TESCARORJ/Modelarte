using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    namespace ByTescaro.ConstrutorApp.Domain.Interfaces
    {
        public interface IObraRetrabalhoRepository : IRepository<ObraRetrabalho>
        {
            Task<List<ObraRetrabalho>> GetByObraIdAsync(long obraId);
        }
    }
}
