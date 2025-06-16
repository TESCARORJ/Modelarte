using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraImagemRepository : IObraImagemRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraImagemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ObraImagem>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraImagem
                .Where(d => d.ObraId == obraId)
                .OrderByDescending(d => d.DataHoraCadastro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ObraImagem>> GetAllAsync()
        {
            return await _context.ObraImagem
                .AsNoTracking()
                .OrderByDescending(d => d.DataHoraCadastro)
                .ToListAsync();
        }

        public async Task<List<ObraImagem>> FindAll(Expression<Func<ObraImagem, bool>> filtro)
        {
            return await _context.ObraImagem
                .Where(filtro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraImagem?> GetByIdAsync(long id)
        {
            return await _context.ObraImagem.FindAsync(id);
        }

        public async Task AddAsync(ObraImagem entity)
        {
            await _context.ObraImagem.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraImagem entity)
        {
            var local = _context.ObraImagem.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraImagem entity)
        {
            _context.ObraImagem.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraImagem.AnyAsync(d => d.Id == id);
        }
    }
}
