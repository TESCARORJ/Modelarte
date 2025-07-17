using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraServicoListaRepository : Repository<ObraServicoLista>, IObraServicoListaRepository
    {
        public ObraServicoListaRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<ObraServicoLista?> GetByIdWithItensAsync(long id)
        {
            return await _dbSet
            .Where(lista => lista.Id == id)
            // Inclui a entidade relacionada "Responsavel"
            .Include(lista => lista.Responsavel)
            // Inclui a coleção de "Itens" e, para cada item, inclui a entidade "Servico"
            .Include(lista => lista.Itens)
                .ThenInclude(item => item.Servico)
            .FirstOrDefaultAsync();
        }

        public async Task<List<ObraServicoLista>> GetByObraIdAsync(long obraId)
        {
            return await _dbSet 
                .Where(l => l.ObraId == obraId)
                .Include(l => l.Responsavel)
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ObraServicoLista?> GetByIdWithItensNoTrackingAsync(long id)
        {
            return await _dbSet
                .AsNoTracking() // Importante: não rastreia a entidade principal
                .Include(o => o.Itens) // Inclui a coleção de itens (também não serão rastreados por herança do AsNoTracking)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}