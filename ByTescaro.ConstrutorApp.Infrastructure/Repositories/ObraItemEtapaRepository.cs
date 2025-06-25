using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaRepository : IObraItemEtapaRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraItemEtapaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraItemEtapa?> GetByIdAsync(long id)
        {
            return await _context.ObraItemEtapa.FindAsync(id);
        }

        public async Task<List<ObraItemEtapa>> GetAllAsync()
        {
            return await _context.ObraItemEtapa.ToListAsync();
        }

        public async Task AddAsync(ObraItemEtapa entity)
        {
            await _context.ObraItemEtapa.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraItemEtapa entity)
        {
            _context.ObraItemEtapa.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraItemEtapa entity)
        {
            var entityToRemove = await _context.ObraItemEtapa.FindAsync(entity.Id);

            if (entityToRemove != null)
            {
                _context.ObraItemEtapa.Remove(entityToRemove);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraItemEtapa.AnyAsync(i => i.Id == id);
        }

        public async Task<List<ObraItemEtapa>> GetByEtapaIdAsync(long obraEtapaId)
        {
            return await _context.ObraItemEtapa
                .Where(i => i.ObraEtapaId == obraEtapaId)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}