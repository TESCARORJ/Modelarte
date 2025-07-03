using ByTescaro.ConstrutorApp.Domain.Entities;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IFuncionarioRepository : IRepository<Funcionario>
{
    Task<List<Funcionario>> ObterAtivosAsync();
    Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids);

    //Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync();
    Task<List<Funcionario>> GetAllIncludingAsync(params Expression<Func<Funcionario, object>>[] includes);

}
