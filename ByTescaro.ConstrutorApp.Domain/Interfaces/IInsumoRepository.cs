using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IInsumoRepository : IRepository<Insumo>
{
    Task<List<Insumo>> ObterAtivosAsync();
    Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids);


}
