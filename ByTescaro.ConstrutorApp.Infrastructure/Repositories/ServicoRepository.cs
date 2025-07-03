// Arquivo: Infrastructure/Repositories/ServicoRepository.cs
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ServicoRepository : Repository<Servico>, IServicoRepository
    {
        public ServicoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Servico>> FindAllAsync(Expression<Func<Servico, bool>> filtro)
        {
            return await _dbSet
                .Where(filtro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Servico>> ObterAtivosAsync()
        {
            return await _dbSet.Where(f => f.Ativo == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            // Adicionada a correção para o aviso de nulabilidade CS8619
            return await _dbSet
                .Where(s => ids.Contains(s.Id) && s.Nome != null)
                .ToDictionaryAsync(s => s.Id, s => s.Nome!);
        }

    }
}