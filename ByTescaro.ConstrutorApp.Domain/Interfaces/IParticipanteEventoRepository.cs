using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IParticipanteEventoRepository : IRepository<ParticipanteEvento>
    {
        Task<ParticipanteEvento> GetByEventoAndUsuarioAsync(long eventoId, long usuarioId);
        Task<IEnumerable<ParticipanteEvento>> GetParticipantesByEventoIdAsync(long eventoId);
    }
}
