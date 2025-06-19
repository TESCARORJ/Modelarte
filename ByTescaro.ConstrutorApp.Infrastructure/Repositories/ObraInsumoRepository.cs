using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraInsumoRepository : IObraInsumoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraInsumoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraInsumo?> GetByIdAsync(long id)
        {
            return await _context.ObraInsumo.FindAsync(id);
        }

        public async Task<List<ObraInsumo>> GetAllAsync()
        {
            return await _context.ObraInsumo.ToListAsync();
        }

        public async Task AddAsync(ObraInsumo entity)
        {
            await _context.ObraInsumo.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraInsumo entity)
        {
            _context.ObraInsumo.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraInsumo entity)
        {
            _context.ObraInsumo.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraInsumo.AnyAsync(i => i.Id == id);
        }

        public async Task<List<ObraInsumo>> GetByListaIdAsync(long listaId)
        {
            return await _context.ObraInsumo
                .Where(i => i.ObraInsumoListaId == listaId)
                .Include(i => i.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Insumo>> GetInsumosDisponiveisAsync(long obraId)
        {
            // Exemplo: retornar insumos que ainda não estão em nenhuma lista dessa obra
            var usados = await _context.ObraInsumo
                .Where(i => i.Lista.ObraId == obraId)
                .Select(i => i.InsumoId)
                .Distinct()
                .ToListAsync();

            return await _context.Insumo
                .Where(i => !usados.Contains(i.Id))
                .Select(i => new Insumo
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
            var nomesItensExecutados = await _context.ObraItemEtapa
                .Where(i => i.ObraEtapa.ObraId == obraId)
                .Select(i => i.Nome.Trim().ToLower())
                .Distinct()
                .ToListAsync();

            // Relacionar com os insumos dos padrões
            var insumos = await _context.ObraItemEtapaPadraoInsumos
                .Include(i => i.Insumo)
                .Include(i => i.ObraItemEtapaPadrao)
                .Where(i => nomesItensExecutados.Contains(i.ObraItemEtapaPadrao.Nome.Trim().ToLower()))
                .Select(i => i.Insumo)
                .Distinct()
                .ToListAsync();

            return insumos;
        }


    }

}