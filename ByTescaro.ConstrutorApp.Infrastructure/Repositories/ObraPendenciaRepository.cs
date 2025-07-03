using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraPendenciaRepository : Repository<ObraPendencia>, IObraPendenciaRepository
    {
        public ObraPendenciaRepository(ApplicationDbContext context) : base(context)
        {
        }

        
        public async Task<List<ObraPendencia>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(r => r.ObraId == obraId)
                .Include(r => r.Responsavel)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
