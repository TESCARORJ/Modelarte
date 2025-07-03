using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FuncaoRepository : Repository<Funcao>, IFuncaoRepository
    {
        public FuncaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Funcao>> ObterTodasAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<Funcao?> ObterPorNomeAsync(string nome)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }
    }
}
