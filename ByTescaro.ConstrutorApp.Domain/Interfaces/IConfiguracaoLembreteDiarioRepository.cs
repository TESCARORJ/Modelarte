using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IConfiguracaoLembreteDiarioRepository : IRepository<ConfiguracaoLembreteDiario>
    {
        Task<IEnumerable<ConfiguracaoLembreteDiario>> GetActiveDailyRemindersAsync();
    }
}
