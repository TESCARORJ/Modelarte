// Em ByTescaro.ConstrutorApp.Infrastructure.Repositories/ConfiguracaoLembreteDiarioRepository.cs
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByTescaro.ConstrutorApp.Infrastructure.Repositories
{
    public class ConfiguracaoLembreteDiarioRepository : Repository<ConfiguracaoLembreteDiario>, IConfiguracaoLembreteDiarioRepository
    {
        public ConfiguracaoLembreteDiarioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConfiguracaoLembreteDiario>> GetActiveDailyRemindersAsync()
        {
            return await _dbSet
                         .Include(c => c.UsuarioCadastro) // Inclui o usuário para que o nome possa ser mapeado
                         .Where(c => c.Ativo == true)
                         .ToListAsync();
        }

        public override async Task<List<ConfiguracaoLembreteDiario>> GetAllAsync()
        {
            return await _dbSet
                         .Include(c => c.UsuarioCadastro) // Inclui o usuário para que o nome possa ser mapeado
                         .ToListAsync();
        }

        public override async Task<ConfiguracaoLembreteDiario?> GetByIdAsync(long id)
        {
            return await _dbSet
                         .Include(c => c.UsuarioCadastro) // Inclui o usuário para que o nome possa ser mapeado
                         .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}