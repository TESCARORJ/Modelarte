using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IPerfilUsuarioRepository : IRepository<PerfilUsuario>
{
    Task<List<PerfilUsuario>> ObterAtivosAsync();
    Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids);



}
