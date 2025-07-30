using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
    {
        public FornecedorRepository(ApplicationDbContext context) : base(context)
        {
        }

     
        public async Task<Fornecedor?> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);
        }

        public async Task<List<Fornecedor>> GetAllIncludingAsync(params Expression<Func<Fornecedor, object>>[] includes)
        {
            IQueryable<Fornecedor> query = _context.Set<Fornecedor>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            query = query.Include(f => f.UsuarioCadastro);

            return await query.ToListAsync();
        }

    }
}
