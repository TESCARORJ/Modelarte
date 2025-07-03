using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraItemEtapaPadraoRepository : Repository<ObraItemEtapaPadrao>,  IObraItemEtapaPadraoRepository
    {
        public ObraItemEtapaPadraoRepository(ApplicationDbContext context) : base(context)
        {
        }

        

        public async Task<List<ObraItemEtapaPadrao>> GetByEtapaPadraoIdAsync(long obraEtapaId)
        {
            return await _dbSet
                .Include(x => x.ObraEtapaPadrao)
                .Where(i => i.ObraEtapaPadraoId == obraEtapaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> JaExisteAsync(string nome, long obraEtapaPadraoId, long idExcluido = 0)
        {
            // Verifica se existe algum item com o mesmo nome na mesma etapa,
            // ignorando o próprio ID do item que está sendo editado (se for o caso).
            return await _dbSet
                .AnyAsync(x => x.Nome == nome &&
                               x.ObraEtapaPadraoId == obraEtapaPadraoId &&
                               x.Id != idExcluido);
        }
    }

}