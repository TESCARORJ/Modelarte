using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class OrcamentoObraRepository : Repository<OrcamentoObra>, IOrcamentoObraRepository
    {
        public OrcamentoObraRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<OrcamentoObra>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(x => x.ObraId == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrcamentoObra?> GetByIdWithItensAsync(long id)
        {
            return await _dbSet
                .Include(x => x.Itens)
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.Id == id);
        }

      
    }
}