using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class OrcamentoRepository : Repository<Orcamento>, IOrcamentoRepository
    {
        public OrcamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Orcamento>> GetByObraAsync(long obraId)
        {
            return await _dbSet
                .Where(o => o.ObraId == obraId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Orcamento?> GetByIdComItensAsync(long id)
        {
            return await _dbSet
                .Include(o => o.Itens)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Orcamento?> GetByIdComItensNoTrackingAsync(long id)
        {
            return await _dbSet
                .AsNoTracking() // Importante: não rastreia a entidade principal
                .Include(o => o.Itens) // Inclui a coleção de itens (também não serão rastreados por herança do AsNoTracking)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

    }
}