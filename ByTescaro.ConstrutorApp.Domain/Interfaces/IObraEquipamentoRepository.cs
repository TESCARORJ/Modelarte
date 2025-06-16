using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraEquipamentoRepository : IRepository<ObraEquipamento>
    {
        Task<List<ObraEquipamento>> GetByObraIdAsync(long obraId);
    }
}
