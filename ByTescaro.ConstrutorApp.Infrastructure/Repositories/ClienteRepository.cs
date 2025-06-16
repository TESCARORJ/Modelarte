using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await _context.Cliente
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<List<Cliente>> FindAll(Expression<Func<Cliente, bool>> filtro)
        {
            return await _context.Cliente.Where(filtro).ToListAsync();
        }

        public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _context.Cliente
                .FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);
        }

        public async Task<Cliente?> GetByIdAsync(long id)
        {
            return await _context.Cliente.FindAsync(id);
        }

        public async Task AddAsync(Cliente entity)
        {
            await _context.Cliente.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Cliente entity)
        {
            _context.Cliente.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Cliente.AnyAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Cliente entity)
        {
            //_context.Cliente.Update(entity);
            //_context.SaveChanges(); // Síncrono!

            var local = _context.Cliente.Local.FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }


        public async Task<List<Cliente>> ObterAtivosAsync()
        {
            return await _context.Cliente
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
