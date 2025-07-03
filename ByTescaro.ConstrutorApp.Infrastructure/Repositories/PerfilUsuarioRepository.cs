using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class PerfilUsuarioRepository : Repository<PerfilUsuario>, IPerfilUsuarioRepository
    {
        public PerfilUsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<List<PerfilUsuario>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Id != 1) 
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<PerfilUsuario>> ObterAtivosAsync()
        {
            return await _dbSet
              .Where(f => f.Ativo == true)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<List<PerfilUsuario>> FindAllAsync(Expression<Func<PerfilUsuario, bool>> filtro)
        {
            return await _dbSet.Where(filtro).AsNoTracking().ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _dbSet
                .Where(f => ids.Contains(f.Id) && f.Nome != null)
                .ToDictionaryAsync(f => f.Id, f => f.Nome!); // Prevenção contra nomes nulos
        }

       
    }
}