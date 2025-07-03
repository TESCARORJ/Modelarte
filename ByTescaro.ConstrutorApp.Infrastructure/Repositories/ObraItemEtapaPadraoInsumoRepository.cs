using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaPadraoInsumoRepository : Repository<ObraItemEtapaPadraoInsumo>, IObraItemEtapaPadraoInsumoRepository
    {
        public ObraItemEtapaPadraoInsumoRepository(ApplicationDbContext context) : base(context)
        {
        }       

        public async Task<List<ObraItemEtapaPadraoInsumo>> GetByItemPadraoIdAsync(long itemPadraoId) =>
            await _dbSet
                .Include(i => i.Insumo)
                .Where(i => i.ObraItemEtapaPadraoId == itemPadraoId)
                .AsNoTracking()
                .ToListAsync();

       
    }
}
