using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories;

public class OrcamentoRepository : IOrcamentoRepository
{
    private readonly ApplicationDbContext _context;

    public OrcamentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Orcamento?> GetByIdAsync(long id)
    {
        return await _context.Orcamento.FindAsync(id);
    }

    public async Task<List<Orcamento>> GetAllAsync()
    {
        return await _context.Orcamento.ToListAsync();
    }

    public async Task AddAsync(Orcamento entity)
    {
        await _context.Orcamento.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Orcamento entity)
    {
        _context.Orcamento.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Orcamento entity)
    {
        _context.Orcamento.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Orcamento.AnyAsync(o => o.Id == id);
    }

    public async Task<List<Orcamento>> GetByObraAsync(long obraId)
    {
        return await _context.Orcamento
            .Where(o => o.ObraId == obraId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Orcamento?> GetByIdComItensAsync(long id)
    {
        return await _context.Orcamento
            .Include(o => o.Itens)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

}
