using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorInsumoRepository : Repository<FornecedorInsumo>, IFornecedorInsumoRepository
    {
        public FornecedorInsumoRepository(ApplicationDbContext context) : base(context)
        {
        }      

        public async Task<List<FornecedorInsumo>> ObterPorFornecedorIdAsync(long fornecedorId)
        {
            return await _dbSet
                .Where(f => f.FornecedorId == fornecedorId)
                .Include(f => f.Fornecedor)
                .Include(f => f.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<FornecedorInsumo>> ObterPorInsumoIdAsync(long insumoId)
        {
            return await _dbSet
                .Where(f => f.InsumoId == insumoId)
                .Include(f => f.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
