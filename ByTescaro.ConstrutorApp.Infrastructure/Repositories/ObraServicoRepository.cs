using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraServicoRepository : IObraServicoRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraServicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ObraServico?> GetByIdAsync(long id)
        {
            return await _context.ObraServico.FindAsync(id);
        }

        public async Task<List<ObraServico>> GetAllAsync()
        {
            return await _context.ObraServico.ToListAsync();
        }

        public async Task AddAsync(ObraServico entity)
        {
            await _context.ObraServico.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ObraServico entity)
        {
            _context.ObraServico.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ObraServico entity)
        {
            _context.ObraServico.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.ObraServico.AnyAsync(i => i.Id == id);
        }

        public async Task<List<ObraServico>> GetByListaIdAsync(long listaId)
        {
            return await _context.ObraServico
                .Where(i => i.ObraServicoListaId == listaId)
                .Include(i => i.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Servico>> GetServicosDisponiveisAsync(long obraId)
        {
            // Exemplo: retornar insumos que ainda não estão em nenhuma lista dessa obra
            var usados = await _context.ObraServico
                .Where(i => i.Lista.ObraId == obraId)
                .Select(i => i.ServicoId)
                .Distinct()
                .ToListAsync();

            return await _context.Servico
                .Where(i => !usados.Contains(i.Id))
                .Select(i => new Servico
                {
                    Id = i.Id,
                    Nome = i.Nome
                })
                .AsNoTracking()
                .ToListAsync();
        }

    }

}