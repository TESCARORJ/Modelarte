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
    public class FuncaoRepository : IFuncaoRepository
    {
        private readonly ApplicationDbContext _context;

        public FuncaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Funcao>> ObterTodasAsync()
        {
            return await _context.Funcao
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<Funcao?> ObterPorNomeAsync(string nome)
        {
            return await _context.Funcao
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }

        public async Task<Funcao?> GetByIdAsync(long id)
        {
            return await _context.Funcao.FindAsync(id);
        }

        public async Task AddAsync(Funcao entity)
        {
            await _context.Funcao.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Funcao entity)
        {
            var local = _context.Funcao.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null) _context.Entry(local).State = EntityState.Detached;

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Funcao entity)
        {
            _context.Funcao.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Funcao>> GetAllAsync()
        {
            return await _context.Funcao
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Funcao.AnyAsync(f => f.Id == id);
        }
    }
}
