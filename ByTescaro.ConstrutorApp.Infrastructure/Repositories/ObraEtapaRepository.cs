using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEtapaRepository : IObraEtapaRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraEtapaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraEtapa?> GetByIdAsync(long id)
        {
            return await _context.ObraEtapa.FindAsync(id);
        }

        public async Task<List<ObraEtapa>> GetAllAsync()
        {
            return await _context.ObraEtapa.ToListAsync();
        }

        public async Task AddAsync(ObraEtapa entity)
        {
            await _context.ObraEtapa.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraEtapa entity)
        {
            // Verifica se já existe a entidade sendo rastreada
            var local = _context.ChangeTracker.Entries<ObraEtapa>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local.Entity).State = EntityState.Detached;
            }

            _context.ObraEtapa.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(ObraEtapa entity)
        {
            var local = _context.ObraEtapa.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached; // evita conflito
            }

            _context.ObraEtapa.Attach(entity); // garante rastreamento
            _context.ObraEtapa.Remove(entity); // marca para remoção

            await _context.SaveChangesAsync();
        }



        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraEtapa.AnyAsync(e => e.Id == id);
        }

        public async Task<List<ObraEtapa>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraEtapa
                .Where(e => e.ObraId == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraEtapa?> GetWithItensAsync(long etapaId)
        {
            return await _context.ObraEtapa
                .Include(e => e.Itens)
                .FirstOrDefaultAsync(e => e.Id == etapaId);
        }
    }

}