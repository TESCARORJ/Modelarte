using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraFornecedorRepository : Repository<ObraFornecedor>, IObraFornecedorRepository
    {
        public ObraFornecedorRepository(ApplicationDbContext context) : base(context)
        {
        }       

        public async Task<List<ObraFornecedor>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}