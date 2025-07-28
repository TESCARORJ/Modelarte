using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ObraRepository : Repository<Obra>, IObraRepository
    {
        public ObraRepository(ApplicationDbContext context) : base(context)
        {
        }       

        public async Task<Obra?> GetByIdWithRelacionamentosAsync(long id)
        {
            return await _dbSet
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
        public async Task<List<Obra>> GetAllWithRelacionamentosAsync()
        {
            return await _dbSet
                .Include(o => o.Projeto)
                    .ThenInclude(p => p.Cliente)
                .Include(o => o.Funcionarios)
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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Obra>> GetByProjetoIdAsync(long projetoId)
        {
            return await _dbSet
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
            return await _dbSet
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
                .FirstOrDefaultAsync(o => o.Etapas.Any(e => e.Itens.Any(i => i.Id == itemId)));
        }

        public async Task<Obra?> GetByEtapaIdAsync(long etapaId)
        {
            return await _dbSet
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
                .FirstOrDefaultAsync(o => o.Etapas.Any(e => e.Id == etapaId));
        }

      

        public async Task AddAsync(Obra entity)
        {
            await _context.Obra.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Obra entity)
        {
            var existingObra = await _dbSet.FindAsync(entity.Id);
            if (existingObra == null)
            {
                throw new InvalidOperationException($"Obra com ID {entity.Id} não encontrada para atualização.");
            }

            _context.Entry(existingObra).CurrentValues.SetValues(entity);
        }

        private IQueryable<Obra> GetQueryWithAllIncludes()
        {
            return _dbSet
                .Include(o => o.Projeto)
                    .ThenInclude(p => p.Cliente)
                .Include(o => o.Funcionarios)
                .Include(o => o.Fornecedores)
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
                .Include(o => o.Etapas)
                    .ThenInclude(e => e.Itens)
                .Include(o => o.Retrabalhos)
                .Include(o => o.Pendencias)
                .Include(o => o.Documentos)
                .Include(o => o.Imagens)
                .AsQueryable();
        }

        public async Task RemoveAsync(long id)
        {
            var obraParaRemover = await GetQueryWithAllIncludes()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obraParaRemover != null)
            {
                foreach (var etapa in obraParaRemover.Etapas.ToList())
                {
                    foreach (var item in etapa.Itens.ToList())
                    {
                        _context.Set<ObraItemEtapa>().Remove(item);
                    }
                    _context.Set<ObraEtapa>().Remove(etapa);
                }

                _dbSet.Remove(obraParaRemover);
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