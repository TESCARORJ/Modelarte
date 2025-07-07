using ByTescaro.ConstrutorApp.Domain.Common;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IRepository<T> where T : EntidadeBase
    {

        Task<T?> GetByIdAsync(long id);
        Task<List<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        //Task<List<T>> GetActivesAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindOneWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindAllWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);


        // Métodos de escrita são síncronos e não retornam Task.
        // A responsabilidade de salvar (Commit) é da Unit of Work.
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }

}