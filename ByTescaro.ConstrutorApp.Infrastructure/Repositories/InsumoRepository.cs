using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly ApplicationDbContext _context;

        public InsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Insumo>> GetAllAsync()
        {
            return await _context.Insumo
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Insumo>> FindAll(Expression<Func<Insumo, bool>> filtro)
        {
            return await _context.Insumo.Where(filtro).ToListAsync();
        }

        public async Task<Insumo?> GetByIdAsync(long id)
        {
            return await _context.Insumo.FindAsync(id);
        }

        public async Task AddAsync(Insumo entity)
        {
            await _context.Insumo.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Insumo entity)
        {
            _context.Insumo.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Insumo.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Insumo entity)
        {

            var local = _context.Insumo.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task<List<Insumo>> ObterAtivosAsync()
        {
            return await _context.Insumo
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Insumo
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }


    }
}
