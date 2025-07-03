using ByTescaro.ConstrutorApp.Domain.Common;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntidadeBase
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(long id) => await _dbSet.FindAsync(id);

        public virtual async Task<List<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();
        
        public virtual async Task<List<T>> GetActivesAsync() => await _dbSet.Where(e => e.Ativo).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        
        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);

        public virtual void Add(T entity)
        {
            // O AddAsync foi removido pois o Add síncrono é suficiente.
            // O EF Core rastreia a entidade em memória.
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);

        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}