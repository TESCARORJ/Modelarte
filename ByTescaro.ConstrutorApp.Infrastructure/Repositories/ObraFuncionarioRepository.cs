using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraFuncionarioRepository : IObraFuncionarioRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraFuncionarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraFuncionario?> GetByIdAsync(long id)
        {
            return await _context.ObraFuncionario.FindAsync(id);
        }

        public async Task<List<ObraFuncionario>> GetAllAsync()
        {
            return await _context.ObraFuncionario.ToListAsync();
        }

        public async Task AddAsync(ObraFuncionario entity)
        {
            await _context.ObraFuncionario.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraFuncionario entity)
        {
            _context.ObraFuncionario.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraFuncionario entity)
        {
            // Evita conflito com EF rastreando múltiplas instâncias de Funcionario
            _context.Entry(entity).Reference(x => x.Funcionario).CurrentValue = null;

            _context.ObraFuncionario.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraFuncionario.AnyAsync(e => e.Id == id);
        }

        public async Task<List<ObraFuncionario>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraFuncionario
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Funcionario)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}