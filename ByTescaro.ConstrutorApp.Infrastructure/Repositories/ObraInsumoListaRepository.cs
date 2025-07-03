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
    public class ObraInsumoListaRepository : Repository<ObraInsumoLista>, IObraInsumoListaRepository
    {
        public ObraInsumoListaRepository(ApplicationDbContext context) : base(context)
        {
        }

       
        public async Task<ObraInsumoLista?> GetByIdWithItensAsync(long id)
        {
            return await _dbSet.Include(l => l.Responsavel)
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Insumo)
                .FirstOrDefaultAsync(l => l.Id == id);
        }


        public async Task<List<ObraInsumoLista>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet.Where(l => l.ObraId == obraId)
                .Include(l => l.Responsavel) 
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
