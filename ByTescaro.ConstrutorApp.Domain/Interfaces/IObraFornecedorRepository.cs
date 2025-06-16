using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraFornecedorRepository : IRepository<ObraFornecedor>
    {
        Task<List<ObraFornecedor>> GetByObraIdAsync(long obraId);
    }
}
