// Arquivo: Infrastructure/Repositories/OrcamentoItemRepository.cs
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class OrcamentoItemRepository : Repository<OrcamentoItem>, IOrcamentoItemRepository
    {
        public OrcamentoItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<OrcamentoItem>> GetByOrcamentoIdAsync(long orcamentoId)
        {
            return await _dbSet 
                .Where(i => i.OrcamentoObraId == orcamentoId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}