using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEtapaPadraoRepository : IObraEtapaPadraoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraEtapaPadraoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraEtapaPadrao?> GetByIdAsync(long id)
        {
            return await _context.ObraEtapaPadrao.FindAsync(id);
        }

        public async Task<List<ObraEtapaPadrao>> GetAllAsync()
        {
            return await _context.ObraEtapaPadrao.ToListAsync();
        }

        public async Task AddAsync(ObraEtapaPadrao entity)
        {
            await _context.ObraEtapaPadrao.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraEtapaPadrao entity)
        {
            _context.ObraEtapaPadrao.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraEtapaPadrao entity)
        {
            _context.ObraEtapaPadrao.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraEtapaPadrao.AnyAsync(e => e.Id == id);
        }

        public async Task<List<ObraEtapaPadrao>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraEtapaPadrao
                .Where(e => e.Id == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraEtapaPadrao?> GetWithItensAsync(long etapaId)
        {
            return await _context.ObraEtapaPadrao
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == etapaId);
        }
    }

}