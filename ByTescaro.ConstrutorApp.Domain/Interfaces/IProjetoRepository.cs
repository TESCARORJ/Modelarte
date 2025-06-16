using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IProjetoRepository : IRepository<Projeto>
{
    void AnexarEntidade(Projeto entidade);
    void RemoverEntidade(Projeto entidade);

}
