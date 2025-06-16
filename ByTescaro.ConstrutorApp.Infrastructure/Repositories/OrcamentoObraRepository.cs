using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class OrcamentoObraRepository : IOrcamentoObraRepository
    {
        private readonly ApplicationDbContext _context;

        public OrcamentoObraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrcamentoObra>> GetByObraIdAsync(long obraId)
        {
            return await _context.OrcamentoObra
                .Where(x => x.ObraId == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrcamentoObra?> GetByIdWithItensAsync(long id)
        {
            return await _context.OrcamentoObra
                .Include(x => x.Itens)
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<OrcamentoObra?> GetByIdAsync(long id)
        {
            return await _context.OrcamentoObra.FindAsync(id);
        }

        public async Task<List<OrcamentoObra>> GetAllAsync()
        {
            return await _context.OrcamentoObra.ToListAsync();
        }

        public async Task AddAsync(OrcamentoObra entity)
        {
            await _context.OrcamentoObra.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrcamentoObra entity)
        {
            _context.OrcamentoObra.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(OrcamentoObra entity)
        {
            _context.OrcamentoObra.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.OrcamentoObra.AnyAsync(x => x.Id == id);
        }
    }
}
