using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IServicoRepository : IRepository<Servico>
{
    Task<List<Servico>> ObterAtivosAsync();
    Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids);


}
