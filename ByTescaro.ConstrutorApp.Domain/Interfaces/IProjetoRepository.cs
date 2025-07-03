using ByTescaro.ConstrutorApp.Domain.Entities;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IProjetoRepository : IRepository<Projeto>
{
    Task<List<T>> GetAllProjectedAsync<T>(Expression<Func<Projeto, T>> projection);
    Task<List<Projeto>> FindAllAsync(Expression<Func<Projeto, bool>> filtro);

    //void AnexarEntidade(Projeto entidade);
    //void RemoverEntidade(Projeto entidade);
    Task<List<Projeto>> GetAllListAsync();
    IQueryable<Projeto> GetQueryable();

}
