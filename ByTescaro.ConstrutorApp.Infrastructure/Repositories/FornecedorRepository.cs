using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        private readonly ApplicationDbContext _context;

        public FornecedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Fornecedor>> GetAllAsync()
        {
            return await _context.Fornecedor
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Fornecedor>> FindAll(Expression<Func<Fornecedor, bool>> filtro)
        {
            return await _context.Fornecedor.Where(filtro).ToListAsync();
        }

        public async Task<Fornecedor?> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _context.Fornecedor
                .FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);
        }

        public async Task<Fornecedor?> GetByIdAsync(long id)
        {
            return await _context.Fornecedor.FindAsync(id);
        }

        public async Task AddAsync(Fornecedor entity)
        {
            await _context.Fornecedor.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Fornecedor entity)
        {
            _context.Fornecedor.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Fornecedor.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Fornecedor entity)
        {
            //_context.Fornecedor.Update(entity);
            //_context.SaveChanges(); // Síncrono!

            var local = _context.Fornecedor.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }


        public async Task<List<Fornecedor>> ObterAtivosAsync()
        {
            return await _context.Fornecedor
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
