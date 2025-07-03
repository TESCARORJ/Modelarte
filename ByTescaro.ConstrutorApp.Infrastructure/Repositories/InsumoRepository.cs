using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class InsumoRepository : Repository<Insumo>, IInsumoRepository
    {
        public InsumoRepository(ApplicationDbContext context) : base(context)
        {
        }       

        public async Task<List<Insumo>> ObterAtivosAsync()
        {
            return await _dbSet
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _dbSet
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }


    }
}
