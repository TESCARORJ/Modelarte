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
    public class ObraInsumoListaRepository : IObraInsumoListaRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraInsumoListaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraInsumoLista?> GetByIdAsync(long id)
        {
            return await _context.ObraInsumoLista.FindAsync(id);
        }

        public async Task<ObraInsumoLista?> GetByIdWithItensAsync(long id)
        {
            return await _context.ObraInsumoLista
                .Include(l => l.Responsavel)
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Insumo)
                .FirstOrDefaultAsync(l => l.Id == id);
        }


        public async Task<List<ObraInsumoLista>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraInsumoLista
                .Where(l => l.ObraId == obraId)
                .Include(l => l.Responsavel) 
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Insumo)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<List<ObraInsumoLista>> GetAllAsync()
        {
            return await _context.ObraInsumoLista.ToListAsync();
        }

        public async Task AddAsync(ObraInsumoLista entity)
        {
            await _context.ObraInsumoLista.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraInsumoLista entity)
        {
            _context.ObraInsumoLista.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraInsumoLista entity)
        {
            _context.ObraInsumoLista.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraInsumoLista.AnyAsync(l => l.Id == id);
        }
    }
}
