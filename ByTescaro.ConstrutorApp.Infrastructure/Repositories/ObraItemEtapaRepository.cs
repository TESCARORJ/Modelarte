using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaRepository : Repository<ObraItemEtapa>, IObraItemEtapaRepository
    {
        public ObraItemEtapaRepository(ApplicationDbContext context) : base(context)
        {
        }

       public async Task<List<ObraItemEtapa>> GetByEtapaIdAsync(long obraEtapaId)
        {
            return await _dbSet
                .Where(i => i.ObraEtapaId == obraEtapaId)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}