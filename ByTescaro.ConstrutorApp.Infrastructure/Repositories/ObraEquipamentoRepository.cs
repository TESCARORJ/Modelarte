using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraEquipamentoRepository : Repository<ObraEquipamento>, IObraEquipamentoRepository
    {
        public ObraEquipamentoRepository(ApplicationDbContext context) : base(context)
        {
        }       

        public async Task<List<ObraEquipamento>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet.Where(e => e.ObraId == obraId)
                .Include(e => e.Equipamento)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}