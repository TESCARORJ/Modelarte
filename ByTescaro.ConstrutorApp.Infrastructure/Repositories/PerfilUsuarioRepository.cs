using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


    namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
    {
        public class PerfilUsuarioRepository : IPerfilUsuarioRepository
        {
            private readonly ApplicationDbContext _context;

            public PerfilUsuarioRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<PerfilUsuario>> GetAllAsync()
            {
                return await _context.PerfilUsuario
                    .AsNoTracking()
                    .Where(p => p.Id != 1) //SA
                    .OrderBy(c => c.Nome)
                    .ToListAsync();
            }

            public async Task<List<PerfilUsuario>> FindAll(Expression<Func<PerfilUsuario, bool>> filtro)
            {
                return await _context.PerfilUsuario.Where(filtro).ToListAsync();
            }


            public async Task<PerfilUsuario?> GetByIdAsync(long id)
            {
                return await _context.PerfilUsuario.FindAsync(id);
            }

            public async Task AddAsync(PerfilUsuario entity)
            {
                await _context.PerfilUsuario.AddAsync(entity);
                await _context.SaveChangesAsync();
            }

            public async Task RemoveAsync(PerfilUsuario entity)
            {
                _context.PerfilUsuario.Remove(entity);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> ExistsAsync(long id)
            {
                return await _context.PerfilUsuario.AnyAsync(c => c.Id == id);
            }

            public async Task UpdateAsync(PerfilUsuario entity)
            {
                //_context.PerfilUsuario.Update(entity);
                //_context.SaveChanges(); // Síncrono!

                var local = _context.PerfilUsuario.Local.FirstOrDefault(e => e.Id == entity.Id);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }

            public void Remove(PerfilUsuario entity)
            {
                _context.PerfilUsuario.Remove(entity);
                _context.SaveChanges(); // Síncrono!
            }


            public async Task<List<PerfilUsuario>> ObterAtivosAsync()
            {
                return await _context.PerfilUsuario
                    .Where(f => f.Ativo == true)
                    .AsNoTracking()
                    .ToListAsync();
            }

            public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
            {
                return await _context.PerfilUsuario
                    .Where(f => ids.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, f => f.Nome);
            }


        }
    }


