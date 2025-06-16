using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }      

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuario.Include(x => x.PerfilUsuario).AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
        }

        public async Task<List<Usuario>> FindAll(Expression<Func<Usuario, bool>> filtro)
        {
            return await _context.Usuario.Where(filtro).ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _context.Usuario.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Usuario entity)
        {
            await _context.Usuario.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Usuario entity)
        {
            _context.Usuario.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Usuario.AnyAsync(p => p.Id == id);
        }       
       
        public async Task<List<Usuario>> ObterAtivosAsync()
        {
            return await _context.Usuario
                .Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public void UpdateDetached(Usuario entity)
        {
            var local = _context.ChangeTracker.Entries<Usuario>()
         .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (local != null)
            {
                // Detacha a entidade que já está sendo rastreada
                _context.Entry(local.Entity).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        
    }
}
