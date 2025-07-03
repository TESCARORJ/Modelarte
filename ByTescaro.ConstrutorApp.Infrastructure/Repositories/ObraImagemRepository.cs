using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraImagemRepository : Repository<ObraImagem>, IObraImagemRepository
    {
        public ObraImagemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ObraImagem>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(d => d.ObraId == obraId)
                .OrderByDescending(d => d.DataHoraCadastro)
                .AsNoTracking()
                .ToListAsync();
        }

       
    }
}
