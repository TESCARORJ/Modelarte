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
    public class ObraServicoListaRepository : IObraServicoListaRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraServicoListaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraServicoLista?> GetByIdAsync(long id)
        {
            return await _context.ObraServicoLista.FindAsync(id);
        }

        public async Task<ObraServicoLista?> GetByIdWithItensAsync(long id)
        {
            return await _context.ObraServicoLista
                .Include(l => l.Responsavel)
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Servico)
                .FirstOrDefaultAsync(l => l.Id == id);
        }


        public async Task<List<ObraServicoLista>> GetByObraIdAsync(long obraId)
        {
            return await _context.ObraServicoLista
                .Where(l => l.ObraId == obraId)
                .Include(l => l.Responsavel) 
                .Include(l => l.Itens)
                    .ThenInclude(i => i.Servico)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<List<ObraServicoLista>> GetAllAsync()
        {
            return await _context.ObraServicoLista.ToListAsync();
        }

        public async Task AddAsync(ObraServicoLista entity)
        {
            await _context.ObraServicoLista.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraServicoLista entity)
        {
            _context.ObraServicoLista.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraServicoLista entity)
        {
            _context.ObraServicoLista.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraServicoLista.AnyAsync(l => l.Id == id);
        }
    }
}
