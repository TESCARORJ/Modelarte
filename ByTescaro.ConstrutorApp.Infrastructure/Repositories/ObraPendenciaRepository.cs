using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraPendenciaRepository : IObraPendenciaRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraPendenciaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraPendencia?> GetByIdAsync(long id)
        {
            return await _context.ObraPendencia
                .Include(r => r.Responsavel)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ObraPendencia>> GetAllAsync()
        {
            return await _context.ObraPendencia
                .Include(r => r.Responsavel)
                .ToListAsync();
        }

        public async Task AddAsync(ObraPendencia entity)
        {
            await _context.ObraPendencia.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraPendencia entity)
        {
            _context.ObraPendencia.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraPendencia entity)
        {
            _context.ObraPendencia.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraPendencia.AnyAsync(r => r.Id == id);
        }

        public async Task<List<ObraPendencia>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraPendencia
                .Where(r => r.ObraId == obraId)
                .Include(r => r.Responsavel)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
