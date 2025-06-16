using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraDocumentoRepository : IObraDocumentoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ObraDocumento>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraDocumento
                .Where(d => d.ObraId == obraId)
                .OrderByDescending(d => d.DataHoraCadastro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ObraDocumento>> GetAllAsync()
        {
            return await _context.ObraDocumento
                .AsNoTracking()
                .OrderByDescending(d => d.DataHoraCadastro)
                .ToListAsync();
        }

        public async Task<List<ObraDocumento>> FindAll(Expression<Func<ObraDocumento, bool>> filtro)
        {
            return await _context.ObraDocumento
                .Where(filtro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraDocumento?> GetByIdAsync(long id)
        {
            return await _context.ObraDocumento.FindAsync(id);
        }

        public async Task AddAsync(ObraDocumento entity)
        {
            await _context.ObraDocumento.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraDocumento entity)
        {
            var local = _context.ObraDocumento.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraDocumento entity)
        {
            _context.ObraDocumento.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraDocumento.AnyAsync(d => d.Id == id);
        }
    }
}
