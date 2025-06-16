using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorInsumoRepository : IFornecedorInsumoRepository
    {
        private readonly ApplicationDbContext _context;

        public FornecedorInsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FornecedorInsumo>> GetAllAsync()
        {
            return await _context.FornecedorInsumo
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Insumo)
                .AsNoTracking()
                .OrderBy(fi => fi.Fornecedor.Nome)
                .ToListAsync();
        }

        public async Task<List<FornecedorInsumo>> FindAll(Expression<Func<FornecedorInsumo, bool>> filtro)
        {
            return await _context.FornecedorInsumo
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Insumo)
                .Where(filtro)
                .ToListAsync();
        }

        public async Task<FornecedorInsumo?> GetByIdAsync(long id)
        {
            return await _context.FornecedorInsumo
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Insumo)
                .FirstOrDefaultAsync(fi => fi.Id == id);
        }

        public async Task AddAsync(FornecedorInsumo entity)
        {
            await _context.FornecedorInsumo.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FornecedorInsumo entity)
        {
            var local = _context.FornecedorInsumo.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(FornecedorInsumo entity)
        {
            _context.FornecedorInsumo.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.FornecedorInsumo.AnyAsync(f => f.Id == id);
        }

        public async Task<List<FornecedorInsumo>> ObterPorFornecedorIdAsync(long fornecedorId)
        {
            return await _context.FornecedorInsumo
                .Where(f => f.FornecedorId == fornecedorId)
                .Include(f => f.Fornecedor)
                .Include(f => f.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<FornecedorInsumo>> ObterPorInsumoIdAsync(long insumoId)
        {
            return await _context.FornecedorInsumo
                .Where(f => f.InsumoId == insumoId)
                .Include(f => f.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
