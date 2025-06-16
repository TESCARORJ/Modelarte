using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraFornecedorRepository : IObraFornecedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraFornecedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraFornecedor?> GetByIdAsync(long id)
        {
            return await _context.ObraFornecedor.FindAsync(id);
        }

        public async Task<List<ObraFornecedor>> GetAllAsync()
        {
            return await _context.ObraFornecedor.ToListAsync();
        }

        public async Task AddAsync(ObraFornecedor entity)
        {
            await _context.ObraFornecedor.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraFornecedor entity)
        {
            _context.ObraFornecedor.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraFornecedor entity)
        {
            // Evita conflito com EF rastreando múltiplas instâncias de Fornecedor
            _context.Entry(entity).Reference(x => x.Fornecedor).CurrentValue = null;

            _context.ObraFornecedor.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraFornecedor.AnyAsync(e => e.Id == id);
        }

        public async Task<List<ObraFornecedor>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraFornecedor
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}