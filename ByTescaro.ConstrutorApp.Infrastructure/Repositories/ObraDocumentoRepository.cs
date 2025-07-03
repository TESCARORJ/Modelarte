using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraDocumentoRepository : Repository<ObraDocumento>, IObraDocumentoRepository
    {
        public ObraDocumentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ObraDocumento>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet.Where(d => d.ObraId == obraId)
                .OrderByDescending(d => d.DataHoraCadastro)
                .AsNoTracking()
                .ToListAsync();
        }

       
    }
}
