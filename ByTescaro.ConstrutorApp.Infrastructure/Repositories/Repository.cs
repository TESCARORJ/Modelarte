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

        // CORRIGIDO: Adicionado AsNoTracking() para que a entidade não seja rastreada se for apenas para leitura.
        // Se você precisar que a entidade seja rastreada para modificações posteriores, remova o AsNoTracking()
        // e use um método Update Detached ou Attach/Entry.
        public virtual async Task<T?> GetByIdAsync(long id) => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        // Nota: FindAsync() rastreia a entidade. Preferimos FirstOrDefaultAsync + AsNoTracking() para este cenário.

        public virtual async Task<T?> GetByIdTrackingAsync(long id)
        { 
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        //public virtual async Task<List<T>> GetActivesAsync() => await _dbSet.Where(e => e.Ativo).AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate); // Adicionado AsNoTracking() aqui também.

        public async Task<T?> FindOneWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet; 

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet; 

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            // Esta é a parte que pode causar o erro se a entidade já estiver rastreada.
            // Para atualizar uma entidade que veio de fora do contexto (como um DTO mapeado de volta para entidade),
            // a abordagem mais segura é:
            // 1. Tentar encontrar a entidade no contexto.
            // 2. Se encontrar, atualizar as propriedades.
            // 3. Se não encontrar, anexar e marcar como Modified.
            // Ou, usar um método no serviço que lida com o rastreamento (UpdateDetached ou similar).

            // Opção 1: Anexar e marcar como Modified (mais comum para DTOs mapeados de volta)
            _context.Entry(entity).State = EntityState.Modified;

            // Opção 2: Se você quiser um controle mais fino e evitar o erro caso já esteja rastreado:
            /*
            var local = _dbSet.Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached; // Desanexa a entidade antiga
            }
            _context.Entry(entity).State = EntityState.Modified; // Anexa a nova
            */
        }

        public virtual void Remove(T entity)
        {
            try
            {
            _dbSet.Remove(entity);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}