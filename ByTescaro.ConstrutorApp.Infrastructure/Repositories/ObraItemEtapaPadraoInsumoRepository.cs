using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaPadraoInsumoRepository : IObraItemEtapaPadraoInsumoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraItemEtapaPadraoInsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraItemEtapaPadraoInsumo?> GetByIdAsync(long id) =>
            await _context.ObraItemEtapaPadraoInsumos.FindAsync(id);

        public async Task<List<ObraItemEtapaPadraoInsumo>> GetByItemPadraoIdAsync(long itemPadraoId) =>
            await _context.ObraItemEtapaPadraoInsumos
                .Include(i => i.Insumo)
                .Where(i => i.ObraItemEtapaPadraoId == itemPadraoId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<ObraItemEtapaPadraoInsumo>> GetAllAsync() =>
            await _context.ObraItemEtapaPadraoInsumos.ToListAsync();

        public async Task AddAsync(ObraItemEtapaPadraoInsumo entity)
        {
            await _context.ObraItemEtapaPadraoInsumos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraItemEtapaPadraoInsumo entity)
        {
            _context.ObraItemEtapaPadraoInsumos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraItemEtapaPadraoInsumo entity)
        {
            _context.ObraItemEtapaPadraoInsumos.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id) =>
            await _context.ObraItemEtapaPadraoInsumos.AnyAsync(e => e.Id == id);
    }
}
