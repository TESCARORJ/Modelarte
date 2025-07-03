using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraRetrabalhoRepository : Repository<ObraRetrabalho>, IObraRetrabalhoRepository
    {
        public ObraRetrabalhoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<ObraRetrabalho?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(r => r.Responsavel)
                .AsNoTracking() 
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public override async Task<List<ObraRetrabalho>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.Responsavel)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ObraRetrabalho>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet 
                .Where(r => r.ObraId == obraId)
                .Include(r => r.Responsavel)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}