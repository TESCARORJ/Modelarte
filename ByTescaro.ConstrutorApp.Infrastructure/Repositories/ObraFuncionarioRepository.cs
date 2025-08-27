using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraFuncionarioRepository : Repository<ObraFuncionario>, IObraFuncionarioRepository
    {
        public ObraFuncionarioRepository(ApplicationDbContext context) : base(context)
        {
        }        

        public async Task<List<ObraFuncionario>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet
                .Where(e => e.ObraId == obraId)
                .Include(e => e.Funcionario)
                    .ThenInclude(f => f.Funcao)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}