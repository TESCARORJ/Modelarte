using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEtapaRepository : Repository<ObraEtapa>, IObraEtapaRepository
    {
        public ObraEtapaRepository(ApplicationDbContext context) : base(context)
        {
        }

        

        public async Task<List<ObraEtapa>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Itens)
                .ToListAsync();
        }

        public async Task<ObraEtapa?> GetWithItensAsync(long etapaId)
        {
            return await _dbSet
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == etapaId);
        }
    }

}