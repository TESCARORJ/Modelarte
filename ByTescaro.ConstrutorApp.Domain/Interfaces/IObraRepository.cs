using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraRepository : IRepository<Obra>
    {
        Task<Obra?> GetByItemEtapaIdAsync(long itemId);
        Task<Obra?> GetByEtapaIdAsync(long etapaId);
        Task<Obra?> GetByIdWithRelacionamentosAsync(long id);

        Task<List<Obra>> GetByProjetoIdAsync(long projetoId);

        void AnexarEntidade<T>(T entidade) where T : class;

        void RemoverEntidade<T>(T entidade) where T : class;

        Task UpdateAsync(Obra obra);
        Task RemoveAsync(long id);
    }
}
