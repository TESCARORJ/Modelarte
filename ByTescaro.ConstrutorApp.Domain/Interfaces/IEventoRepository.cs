using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IEventoRepository : IRepository<Evento>
    {
        Task<IEnumerable<Evento>> GetEventosByUsuarioIdAsync(long usuarioId);
        Task<IEnumerable<Evento>> GetEventosByDateRangeAsync(DateTime startDate, DateTime endDate, long? userId = null);
    }
}
