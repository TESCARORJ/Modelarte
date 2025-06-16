using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraFuncionarioRepository : IRepository<ObraFuncionario>
    {
        Task<List<ObraFuncionario>> GetByObraIdAsync(long obraId);
    }
}
