using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class EquipamentoRepository : Repository<Equipamento>, IEquipamentoRepository
    {
        public EquipamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Equipamento>> ObterAtivosAsync()
        {
            return await _dbSet.Where(f => f.Ativo == true).AsNoTracking().ToListAsync();
        }

        public async Task<Dictionary<long, string>> ObterNomesPorIdsAsync(IEnumerable<long> ids)
        {
            return await _dbSet
                .Where(f => ids.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.Nome);
        }

        //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        //{
        //    var alocados = await _dbSet
        //        .Where(pf => pf.DataFimUso == null)
        //        .Select(pf => pf.EquipamentoId)
        //        .Distinct()
        //        .CountAsync();

        //    var total = await _context.Equipamento.CountAsync();
        //    var naoAlocados = total - alocados;

        //    return (Alocados: alocados, NaoAlocados: naoAlocados);
        //}

    }
}

