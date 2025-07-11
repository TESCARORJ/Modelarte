using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByTescaro.ConstrutorApp.Domain.Enums; // Adicionado para acessar o enum Visibilidade

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class EventoRepository : Repository<Evento>, IEventoRepository
    {
        public EventoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Evento>> GetEventosByUsuarioIdAsync(long usuarioId)
        {
            return await _dbSet
                .Include(e => e.Participantes)
                    .ThenInclude(pe => pe.Usuario)
                .Include(e => e.Criador)
                .Where(e =>
                    e.CriadorId == usuarioId || // O criador sempre vê seu próprio evento (Privado, Publico, SomenteConvidados)
                    e.Visibilidade == Visibilidade.Publico || // QUALQUER usuário pode ver eventos públicos
                    (e.Visibilidade == Visibilidade.SomenteConvidados && e.Participantes.Any(pe => pe.UsuarioId == usuarioId)) // Usuários convidados veem eventos "Somente Convidados"
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> GetEventosByDateRangeAsync(DateTime startDate, DateTime endDate, long? userId = null)
        {
            var query = _dbSet
                .Include(e => e.Participantes)
                    .ThenInclude(pe => pe.Usuario)
                .Include(e => e.Criador)
                .Where(e => (e.DataHoraInicio <= endDate && e.DataHoraFim >= startDate));

            if (userId.HasValue)
            {
                // Para a busca por range de datas, precisamos decidir se queremos incluir eventos públicos para o userId especificado.
                // A lógica aqui seria um pouco diferente, talvez mais genérica para um calendário de visualização.
                // Por enquanto, mantenho a lógica original baseada em userId se ele for fornecido,
                // mas você pode estender isso para também incluir eventos públicos no range.
                query = query.Where(e => e.CriadorId == userId.Value || e.Participantes.Any(p => p.UsuarioId == userId.Value));
            }
            // NOTA: Se esta busca for para um calendário público ou um "Todos os Eventos", você pode remover o filtro de userId.
            // Se for para um usuário específico, então a visibilidade também deve ser aplicada aqui.

            return await query.ToListAsync();
        }
    }
}