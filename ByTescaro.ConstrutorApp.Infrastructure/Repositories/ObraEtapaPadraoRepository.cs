using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEtapaPadraoRepository : Repository<ObraEtapaPadrao>, IObraEtapaPadraoRepository
    {
        public ObraEtapaPadraoRepository(ApplicationDbContext context) : base(context)
        {
        }        

        public async Task<List<ObraEtapaPadrao>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(e => e.Id == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraEtapaPadrao?> GetWithItensAsync(long etapaId)
        {
            return await _dbSet
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == etapaId);
        }
    }

}