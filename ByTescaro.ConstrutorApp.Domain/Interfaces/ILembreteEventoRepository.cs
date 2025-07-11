using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface ILembreteEventoRepository : IRepository<LembreteEvento>
    {
        Task<IEnumerable<LembreteEvento>> GetLembretesPendentesParaEnvioAsync(DateTime agora);
        Task<IEnumerable<LembreteEvento>> GetLembretesByEventoIdAsync(long eventoId);
    }
}
