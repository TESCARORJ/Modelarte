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
                .Include(e => e.UsuarioCadastro)
                .Where(e =>
                    e.UsuarioCadastroId == usuarioId || // O criador sempre vê seu próprio evento (Privado, Publico, SomenteConvidados)
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
                .Include(e => e.UsuarioCadastro)
                .Where(e => (e.DataHoraInicio <= endDate && e.DataHoraFim >= startDate));

            if (userId.HasValue)
            {
                // Para a busca por range de datas, precisamos decidir se queremos incluir eventos públicos para o userId especificado.
                // A lógica aqui seria um pouco diferente, talvez mais genérica para um calendário de visualização.
                // Por enquanto, mantenho a lógica original baseada em userId se ele for fornecido,
                // mas você pode estender isso para também incluir eventos públicos no range.
                query = query.Where(e => e.UsuarioCadastroId == userId.Value || e.Participantes.Any(p => p.UsuarioId == userId.Value));
            }
            // NOTA: Se esta busca for para um calendário público ou um "Todos os Eventos", você pode remover o filtro de userId.
            // Se for para um usuário específico, então a visibilidade também deve ser aplicada aqui.

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Evento>> GetEventosWithParticipantesAndUsuariosByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Ajustando endDate para ser o final do dia de 'endDate' (23:59:59.999...)
            // Isso garante que eventos que terminam exatamente no final do dia sejam incluídos.
            // A sua chamada no DailyReminderService já usa .AddDays(1).AddTicks(-1), que é o correto.
            // var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1); // Comentado, pois já é tratado na chamada do serviço.

            return await _dbSet
                         .Include(e => e.Participantes) // Inclui os participantes do evento
                            .ThenInclude(p => p.Usuario) // Para cada participante, inclui o objeto Usuario
                         .Where(e =>
                            // O evento começa antes ou no final do período E
                            // O evento termina depois ou no início do período
                            e.DataHoraInicio <= endDate && e.DataHoraFim >= startDate
                            ||
                            // OU se for um evento recorrente, verifica se alguma ocorrência se sobrepõe
                            // Esta lógica de recorrência é mais complexa e depende de como você gera as ocorrências.
                            // Por simplicidade, se EhRecorrente for true, podemos considerar que ele "ocorre" se a data inicial se encaixa,
                            // ou se a DataFimRecorrencia ainda não passou e a frequência engloba o período.
                            // Para um tratamento completo de recorrência, você precisaria de uma função que "expanda" as ocorrências.
                            // POR ENQUANTO, VAMOS FOCAR NA LÓGICA DE OVERLAP SIMPLES PARA DATETIMEINICIO/FIM
                            // E AVISAR SOBRE A RECORRÊNCIA.
                            (e.IsRecorrente && e.DataFimRecorrencia >= startDate.Date) // Se é recorrente e a recorrência ainda é válida
                         )
                         .ToListAsync();
        }

    }
}