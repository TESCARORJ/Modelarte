using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class EquipamentoRepository : IEquipamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public EquipamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Equipamento>> GetAllAsync()
        {
            return await _context.Equipamento
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Equipamento>> FindAll(Expression<Func<Equipamento, bool>> filtro)
        {
            return await _context.Equipamento.Where(filtro).ToListAsync();
        }

        public async Task<Equipamento?> GetByIdAsync(long id)
        {
            return await _context.Equipamento.FindAsync(id);
        }

        public async Task AddAsync(Equipamento entity)
        {
            await _context.Equipamento.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Equipamento entity)
        {
            _context.Equipamento.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Equipamento.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Equipamento entity)
        {
            //_context.Equipamento.Update(entity);
            //_context.SaveChanges(); // Síncrono!

            var local = _context.Equipamento.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task<List<Equipamento>> ObterAtivosAsync()
        {
            return await _context.Equipamento
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _context.Equipamento
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }

        public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        {
            var alocados = await _context.ObraEquipamento
                .Where(pf => pf.DataFimUso == null)
                .Select(pf => pf.EquipamentoId)
                .Distinct()
                .CountAsync();

            var total = await _context.Equipamento.CountAsync();
            var naoAlocados = total - alocados;

            return (Alocados: alocados, NaoAlocados: naoAlocados);
        }

    }
}

