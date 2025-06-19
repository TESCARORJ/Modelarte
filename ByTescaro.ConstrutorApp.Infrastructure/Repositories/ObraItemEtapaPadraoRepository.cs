using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaPadraoRepository : IObraItemEtapaPadraoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraItemEtapaPadraoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraItemEtapaPadrao?> GetByIdAsync(long id)
        {
            return await _context.ObraItemEtapaPadrao
                .Include(x => x.ObraEtapaPadrao)
                .Include(x => x.Insumos)
                    .ThenInclude(i => i.Insumo)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<List<ObraItemEtapaPadrao>> GetAllAsync()
        {
            return await _context.ObraItemEtapaPadrao.Include(x => x.ObraEtapaPadrao).ToListAsync();
        }

        public async Task AddAsync(ObraItemEtapaPadrao entity)
        {
            await _context.ObraItemEtapaPadrao.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraItemEtapaPadrao entity)
        {
            _context.ObraItemEtapaPadrao.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraItemEtapaPadrao entity)
        {
            _context.ObraItemEtapaPadrao.Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraItemEtapaPadrao.AnyAsync(i => i.Id == id);
        }

        public async Task<List<ObraItemEtapaPadrao>> GetByEtapaPadraoIdAsync(long obraEtapaId)
        {
            return await _context.ObraItemEtapaPadrao
                .Include(x => x.ObraEtapaPadrao)
                .Where(i => i.ObraEtapaPadraoId == obraEtapaId)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}