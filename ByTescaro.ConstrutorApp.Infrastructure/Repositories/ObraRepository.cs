using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraRepository : IObraRepository
    {
        private readonly ApplicationDbContext _context;

        public ObraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Obra?> GetByIdAsync(long id)
        {
            return await _context.Obra.Include(x => x.Projeto).ThenInclude(y => y.Cliente).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Obra?> GetByIdWithRelacionamentosAsync(long id)
        {
            return await _context.Obra
                .Include(o => o.Funcionarios)
                .Include(o => o.Fornecedores)
                .Include(o => o.Insumos)
                .Include(o => o.ListasInsumo)
                    .ThenInclude(l => l.Responsavel)
                .Include(o => o.ListasInsumo)
                    .ThenInclude(l => l.Itens)
                        .ThenInclude(i => i.Insumo)
                .Include(o => o.ListasServico)
                    .ThenInclude(l => l.Responsavel)
                .Include(o => o.ListasServico)
                    .ThenInclude(l => l.Itens)
                        .ThenInclude(i => i.Servico)
                .Include(o => o.Equipamentos)
                .Include(o => o.Etapas).ThenInclude(e => e.Itens)
                .Include(o => o.Retrabalhos)
                .Include(o => o.Pendencias)
                .Include(o => o.Documentos)
                .Include(o => o.Imagens)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Obra>> GetByProjetoIdAsync(long projetoId)
        {
            return await _context.Obra
                .Include(o => o.Funcionarios)
                .Include(o => o.Fornecedores)
                .Include(o => o.Insumos)
                .Include(o => o.ListasInsumo)
                    .ThenInclude(l => l.Responsavel)
                .Include(o => o.ListasInsumo)
                    .ThenInclude(l => l.Itens)
                        .ThenInclude(i => i.Insumo)
                .Include(o => o.ListasServico)
                    .ThenInclude(l => l.Responsavel)
                .Include(o => o.ListasServico)
                    .ThenInclude(l => l.Itens)
                        .ThenInclude(i => i.Servico)
                .Include(o => o.Equipamentos)
                .Include(o => o.Etapas).ThenInclude(e => e.Itens)
                .Include(o => o.Retrabalhos)
                .Include(o => o.Pendencias)
                .Include(o => o.Documentos)
                .Include(o => o.Imagens)
                .Where(o => o.ProjetoId == projetoId)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Obra?> GetByItemEtapaIdAsync(long itemId)
        {
            return await _context.Obra
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
                .FirstOrDefaultAsync(o => o.Etapas.Any(e => e.Itens.Any(i => i.Id == itemId)));
        }

        public async Task<Obra?> GetByEtapaIdAsync(long etapaId)
        {
            return await _context.Obra
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
                .FirstOrDefaultAsync(o => o.Etapas.Any(e => e.Id == etapaId));
        }

        public async Task<List<Obra>> GetAllAsync()
        {
            return await _context.Obra.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Obra entity)
        {
            await _context.Obra.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Obra entity)
        {
            var existing = await _context.Obra.FindAsync(entity.Id);
            if (existing == null)
                throw new InvalidOperationException("Obra não encontrada para atualização.");

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }


        public async Task RemoveAsync(Obra entity)
        {
            var existente = await _context.Obra
                .Include(o => o.Funcionarios)
                .Include(o => o.Insumos)
                .Include(o => o.ListasInsumo).ThenInclude(e => e.Itens)
                .Include(o => o.Equipamentos)
                .Include(o => o.Etapas).ThenInclude(e => e.Itens)
                .Include(o => o.Retrabalhos)
                .Include(o => o.Pendencias)
                .Include(o => o.Documentos)
                .Include(o => o.Imagens)
                .FirstOrDefaultAsync(o => o.Id == entity.Id);

            if (existente != null)
            {
                // Remove filhos primeiro se não tiver cascade
                foreach (var etapa in existente.Etapas.ToList())
                {
                    foreach (var item in etapa.Itens.ToList())
                        _context.ObraItemEtapa.Remove(item);

                    _context.ObraEtapa.Remove(etapa);
                }

                _context.Obra.Remove(existente);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Obra.AnyAsync(o => o.Id == id);
        }

        public void AnexarEntidade<T>(T entidade) where T : class
        {
            _context.Set<T>().Attach(entidade);
        }

        public void RemoverEntidade<T>(T entidade) where T : class
        {
            _context.Set<T>().Remove(entidade);
        }
    }

}