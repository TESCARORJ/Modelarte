using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraServicoRepository : Repository<ObraServico>, IObraServicoRepository
    {
        public ObraServicoRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<List<ObraServico>> GetByListaIdAsync(long listaId)
        {
            return await _dbSet 
                .Where(i => i.ObraServicoListaId == listaId)
                .Include(i => i.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Servico>> GetServicosDisponiveisAsync(long obraId)
        {
            var servicosUsadosIds = await _dbSet
                .Where(i => i.Lista.ObraId == obraId)
                .Select(i => i.ServicoId)
                .Distinct()
                .ToListAsync();

            return await _context.Set<Servico>()
                .Where(s => !servicosUsadosIds.Contains(s.Id))
                .Select(s => new Servico 
                {
                    Id = s.Id,
                    Nome = s.Nome
                })
                .AsNoTracking()
                .ToListAsync();
        }

        
    }
}