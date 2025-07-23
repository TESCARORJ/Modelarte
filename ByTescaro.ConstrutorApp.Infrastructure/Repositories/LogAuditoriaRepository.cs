using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class LogAuditoriaRepository : ILogAuditoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public LogAuditoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task RegistrarAsync(LogAuditoria log) // Não precisa ser async agora
        {
            // APENAS adiciona o log ao Change Tracker do EF Core.
            // Ele será salvo quando o Unit of Work for comitado.
            _context.LogAuditoria.Add(log);

            // REMOVA A LINHA ABAIXO
            // await _context.SaveChangesAsync(); 

            return Task.CompletedTask;
        }

        public async Task<List<LogAuditoria>> ObterTodosAsync()
        {
            var logs = _context.LogAuditoria
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
            return await logs;
        }
    }
}

