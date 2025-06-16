using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEquipamentoRepository : IObraEquipamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraEquipamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraEquipamento?> GetByIdAsync(long id)
        {
            return await _context.ObraEquipamento.FindAsync(id);
        }

        public async Task<List<ObraEquipamento>> GetAllAsync()
        {
            return await _context.ObraEquipamento.ToListAsync();
        }

        public async Task AddAsync(ObraEquipamento entity)
        {
            await _context.ObraEquipamento.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraEquipamento entity)
        {
            _context.ObraEquipamento.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraEquipamento entity)
        {
            _context.ObraEquipamento.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraEquipamento.AnyAsync(e => e.Id == id);
        }

        public async Task<List<ObraEquipamento>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraEquipamento
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Equipamento)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}