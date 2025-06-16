using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorServicoRepository : IFornecedorServicoRepository
    {
        private readonly ApplicationDbContext _context;

        public FornecedorServicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FornecedorServico>> GetAllAsync()
        {
            return await _context.FornecedorServico
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Servico)
                .AsNoTracking()
                .OrderBy(fi => fi.Fornecedor.Nome)
                .ToListAsync();
        }

        public async Task<List<FornecedorServico>> FindAll(Expression<Func<FornecedorServico, bool>> filtro)
        {
            return await _context.FornecedorServico
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Servico)
                .Where(filtro)
                .ToListAsync();
        }

        public async Task<FornecedorServico?> GetByIdAsync(long id)
        {
            return await _context.FornecedorServico
                .Include(fi => fi.Fornecedor)
                .Include(fi => fi.Servico)
                .FirstOrDefaultAsync(fi => fi.Id == id);
        }

        public async Task AddAsync(FornecedorServico entity)
        {
            await _context.FornecedorServico.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FornecedorServico entity)
        {
            var local = _context.FornecedorServico.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
                _context.Entry(local).State = EntityState.Detached;

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(FornecedorServico entity)
        {
            _context.FornecedorServico.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.FornecedorServico.AnyAsync(f => f.Id == id);
        }

        public async Task<List<FornecedorServico>> ObterPorFornecedorIdAsync(long fornecedorId)
        {
            return await _context.FornecedorServico
                .Where(f => f.FornecedorId == fornecedorId)
                .Include(f => f.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<FornecedorServico>> ObterPorServicoIdAsync(long insumoId)
        {
            return await _context.FornecedorServico
                .Where(f => f.ServicoId == insumoId)
                .Include(f => f.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
