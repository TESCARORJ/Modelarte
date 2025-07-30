using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FuncionarioRepository : Repository<Funcionario>, IFuncionarioRepository
    {
        public FuncionarioRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<List<Funcionario>> ObterAtivosAsync()
        {
            return await _dbSet.Where(f => f.DataDemissao == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
    
            return await _dbSet.Where(f => ids.Contains(f.Id) && f.Nome != null)
                               .ToDictionaryAsync(f => f.Id, f => f.Nome!);
        }
        //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        //{
        //    var alocados = await _dbSet.Where(pf => pf. == null)
        //        .Select(pf => pf.FuncionarioId)
        //        .Distinct()
        //        .CountAsync();

        //    var total = await _context.Funcionario.CountAsync();
        //    var naoAlocados = total - alocados;

        //    return (Alocados: alocados, NaoAlocados: naoAlocados);
        //}

        public async Task<List<Funcionario>> GetAllIncludingAsync(Expression<Func<Funcionario, bool>>? predicate = null, params Expression<Func<Funcionario, object>>[] includes)
        {
            IQueryable<Funcionario> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            query = query.Include(f => f.UsuarioCadastro);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Funcionario?> GetByIdWithEnderecoAsync(long id)
        {
            return await _dbSet
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


    }
}

