using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraImagemRepository : IRepository<ObraImagem>
    {
        Task<List<ObraImagem>> GetByObraIdAsync(long obraId);
    }
}
