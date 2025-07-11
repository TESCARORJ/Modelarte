using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ParticipanteEventoRepository : Repository<ParticipanteEvento>, IParticipanteEventoRepository
    {
        public ParticipanteEventoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ParticipanteEvento?> GetByEventoAndUsuarioAsync(long eventoId, long usuarioId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pe => pe.EventoId == eventoId && pe.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<ParticipanteEvento>> GetParticipantesByEventoIdAsync(long eventoId)
        {
            return await _dbSet
                .Include(pe => pe.Usuario)
                .Where(pe => pe.EventoId == eventoId)
                .ToListAsync();
        }
    }
}