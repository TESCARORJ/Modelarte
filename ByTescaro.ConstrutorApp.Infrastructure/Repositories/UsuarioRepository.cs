// Arquivo: Infrastructure/Repositories/UsuarioRepository.cs
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Usuario?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(u => u.PerfilUsuario)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public override async Task<List<Usuario>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.PerfilUsuario)
                .AsNoTracking()
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }

        public async Task<List<Usuario>> FindAllAsync(Expression<Func<Usuario, bool>> filtro)
        {
            return await _dbSet
                .Where(filtro)
                .Include(u => u.PerfilUsuario)
                .AsNoTracking()
                .ToListAsync();
        }

        public void UpdateDetached(Usuario entity)
        {
           
            var local = _context.ChangeTracker.Entries<Usuario>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local.Entity).State = EntityState.Detached;
            }

            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<Usuario?> ObterPorEmailComPerfilAsync(string email)
        {
            return await _dbSet
                .AsNoTracking() // <--- CRÍTICO: Garante que o objeto não é rastreado
                .Include(u => u.PerfilUsuario) // Inclua se o PerfilUsuario for usado em algum mapeamento de DTO na auditoria
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

    }
}