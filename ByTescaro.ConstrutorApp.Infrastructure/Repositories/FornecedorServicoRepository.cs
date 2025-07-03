using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorServicoRepository : Repository<FornecedorServico>, IFornecedorServicoRepository
    {
        public FornecedorServicoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<FornecedorServico>> ObterPorFornecedorIdAsync(long fornecedorId)
        {
            return await _dbSet
                .Where(f => f.FornecedorId == fornecedorId)
                .Include(f => f.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<FornecedorServico>> ObterPorServicoIdAsync(long insumoId)
        {
            return await _dbSet
                .Where(f => f.ServicoId == insumoId)
                .Include(f => f.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
