using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraInsumoRepository : Repository<ObraInsumo>, IObraInsumoRepository
    {
        public ObraInsumoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ObraInsumo>> GetByListaIdAsync(long listaId)
        {
            // Este método já estava correto, usando _dbSet.
            return await _dbSet
                .Where(i => i.ObraInsumoListaId == listaId)
                .Include(i => i.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Insumo>> GetInsumosDisponiveisAsync(long obraId)
        {
            // O início da query usa _dbSet para encontrar os insumos já utilizados na obra.
            var insumosUsadosIds = await _dbSet
                .Where(i => i.Lista.ObraId == obraId)
                .Select(i => i.InsumoId)
                .Distinct()
                .ToListAsync();

            // A segunda parte busca em outra tabela. Usamos _context.Set<T>() para consistência.
            return await _context.Set<Insumo>()
                .Where(i => !insumosUsadosIds.Contains(i.Id))
                .Select(i => new Insumo // Seleciona apenas os campos necessários
                {
                    Id = i.Id,
                    Nome = i.Nome,
                    UnidadeMedida = i.UnidadeMedida
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Insumo>> GetInsumosPorPadraoObraAsync(long obraId)
        {
            // Obter nomes dos itens da obra
            var nomesItensExecutados = await _context.Set<ObraItemEtapa>()
                .Where(i => i.ObraEtapa.ObraId == obraId)
                .Select(i => i.Nome.Trim().ToLower())
                .Distinct()
                .ToListAsync();

            // Relacionar com os insumos dos padrões
            var insumos = await _context.Set<ObraItemEtapaPadraoInsumo>()
                .Where(i => nomesItensExecutados.Contains(i.ObraItemEtapaPadrao.Nome.Trim().ToLower()))
                .Select(i => i.Insumo) // Seleciona a entidade Insumo navegada
                .Distinct()
                .ToListAsync();

            return insumos;
        }
    }
}