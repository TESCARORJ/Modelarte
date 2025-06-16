using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories;

public class OrcamentoItemRepository : IOrcamentoItemRepository
{
    private readonly ApplicationDbContext _context;

    public OrcamentoItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrcamentoItem?> GetByIdAsync(long id)
    {
        return await _context.OrcamentoItem.FindAsync(id);
    }

    public async Task<List<OrcamentoItem>> GetAllAsync()
    {
        return await _context.OrcamentoItem.ToListAsync();
    }

    public async Task AddAsync(OrcamentoItem entity)
    {
        await _context.OrcamentoItem.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrcamentoItem entity)
    {
        _context.OrcamentoItem.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(OrcamentoItem entity)
    {
        _context.OrcamentoItem.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.OrcamentoItem.AnyAsync(o => o.Id == id);
    }

    public async Task<List<OrcamentoItem>> GetByOrcamentoIdAsync(long orcamentoId)
    {
        return await _context.OrcamentoItem
            .Where(i => i.OrcamentoObraId == orcamentoId)
            .AsNoTracking()
            .ToListAsync();
    }
}
