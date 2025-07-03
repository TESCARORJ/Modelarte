using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class OrcamentoRepository : Repository<Orcamento>, IOrcamentoRepository
    {
        public OrcamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Orcamento>> GetByObraAsync(long obraId)
        {
            return await _dbSet
                .Where(o => o.ObraId == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Orcamento?> GetByIdComItensAsync(long id)
        {
            return await _dbSet
                .Include(o => o.Itens)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

    }
}