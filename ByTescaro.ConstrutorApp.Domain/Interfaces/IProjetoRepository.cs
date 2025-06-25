using ByTescaro.ConstrutorApp.Domain.Entities;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IProjetoRepository : IRepository<Projeto>
{
    void AnexarEntidade(Projeto entidade);
    void RemoverEntidade(Projeto entidade);
    Task<List<Projeto>> GetAllListAsync();
    IQueryable<Projeto> GetQueryable();

}
