using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IEquipamentoRepository : IRepository<Equipamento>
{
    Task<List<Equipamento>> ObterAtivosAsync();
    Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids);
    //Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync();



}
