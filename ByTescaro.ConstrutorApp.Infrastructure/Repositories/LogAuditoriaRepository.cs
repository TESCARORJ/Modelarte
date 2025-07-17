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

        public async Task RegistrarAsync(LogAuditoria log)
        {
            _context.LogAuditoria.Add(log);
            await _context.SaveChangesAsync();
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

