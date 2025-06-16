using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly ApplicationDbContext _context;

        public ServicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Servico>> GetAllAsync()
        {
            return await _context.Servico
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Servico>> FindAll(Expression<Func<Servico, bool>> filtro)
        {
            return await _context.Servico.Where(filtro).ToListAsync();
        }

        public async Task<Servico?> GetByIdAsync(long id)
        {
            return await _context.Servico.FindAsync(id);
        }

        public async Task AddAsync(Servico entity)
        {
            await _context.Servico.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Servico entity)
        {
            _context.Servico.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Servico.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Servico entity)
        {

            var local = _context.Servico.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task<List<Servico>> ObterAtivosAsync()
        {
            return await _context.Servico
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Servico
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }


    }
}
