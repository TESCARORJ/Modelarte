using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraRetrabalhoRepository : IObraRetrabalhoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraRetrabalhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraRetrabalho?> GetByIdAsync(long id)
        {
            return await _context.ObraRetrabalho
                .Include(r => r.Responsavel)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<ObraRetrabalho>> GetAllAsync()
        {
            return await _context.ObraRetrabalho
                .Include(r => r.Responsavel)
                .ToListAsync();
        }

        public async Task AddAsync(ObraRetrabalho entity)
        {
            await _context.ObraRetrabalho.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraRetrabalho entity)
        {
            _context.ObraRetrabalho.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraRetrabalho entity)
        {
            _context.ObraRetrabalho.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraRetrabalho.AnyAsync(r => r.Id == id);
        }

        public async Task<List<ObraRetrabalho>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraRetrabalho
                .Where(r => r.ObraId == obraId)
                .Include(r => r.Responsavel)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
