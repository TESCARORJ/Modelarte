using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories;

public class ProjetoRepository : IProjetoRepository
{
    private readonly ApplicationDbContext _context;

    public ProjetoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Projeto?> GetByIdAsync(long id)
    {
        return await _context.Projeto
            .Include(p => p.Cliente)
            .Include(p => p.Obras).ThenInclude(o => o.Etapas).ThenInclude(x => x.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Projeto>> GetAllAsync()
    {
        return await _context.Projeto
            .Include(p => p.Cliente)
            .Include(p => p.Obras)
                .ThenInclude(o => o.Funcionarios)
            .Include(p => p.Obras)
                .ThenInclude(o => o.Insumos)
            .Include(p => p.Obras)
                .ThenInclude(o => o.Equipamentos)
            .Include(p => p.Obras)
                .ThenInclude(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<List<Projeto>> FindAll(Expression<Func<Projeto, bool>> filtro)
    {
        return await _context.Projeto.Where(filtro).ToListAsync();
    }

    public async Task AddAsync(Projeto entity)
    {
        await _context.Projeto.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Projeto entity)
    {
        _context.Projeto.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Projeto entity)
    {
        _context.Projeto.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Projeto.AnyAsync(p => p.Id == id);
    }

    public void AnexarEntidade(Projeto entidade)
    {
        _context.Set<Projeto>().Attach(entidade);
    }

    public void RemoverEntidade(Projeto entidade)
    {
        _context.Set<Projeto>().Remove(entidade);
    }
}

