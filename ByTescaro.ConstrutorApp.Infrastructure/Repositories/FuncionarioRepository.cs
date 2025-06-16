using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly ApplicationDbContext _context;

        public FuncionarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Funcionario>> GetAllAsync()
        {
            return await _context.Funcionario
                .AsNoTracking()
                .Include(x => x.Funcao)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Funcionario>> FindAll(Expression<Func<Funcionario, bool>> filtro)
        {
            return await _context.Funcionario.Where(filtro).ToListAsync();
        }


        public async Task<Funcionario?> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _context.Funcionario
                .FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);
        }

        public async Task<Funcionario?> GetByIdAsync(long id)
        {
            return await _context.Funcionario.Include(x => x.Funcao).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(Funcionario entity)
        {
            await _context.Funcionario.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Funcionario entity)
        {
            _context.Funcionario.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Funcionario.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Funcionario entity)
        {
            //_context.Funcionario.Update(entity);
            //_context.SaveChanges(); // Síncrono!

            var local = _context.Funcionario.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task<List<Funcionario>> ObterAtivosAsync()
        {
            return await _context.Funcionario
                .Where(f => f.DataDemissao == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Funcionario
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }

        public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        {
            var alocados = await _context.ObraFuncionario
                .Where(pf => pf.DataFim == null)
                .Select(pf => pf.FuncionarioId)
                .Distinct()
                .CountAsync();

            var total = await _context.Funcionario.CountAsync();
            var naoAlocados = total - alocados;

            return (Alocados: alocados, NaoAlocados: naoAlocados);
        }

        public async Task<List<Funcionario>> GetAllIncludingAsync(params Expression<Func<Funcionario, object>>[] includes)
        {
            IQueryable<Funcionario> query = _context.Set<Funcionario>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }


    }
}

