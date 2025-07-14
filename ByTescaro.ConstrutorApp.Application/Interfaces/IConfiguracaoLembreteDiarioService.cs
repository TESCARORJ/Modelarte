using ByTescaro.ConstrutorApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IConfiguracaoLembreteDiarioService
    {
        Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetAllAsync();
        Task<ConfiguracaoLembreteDiarioDto?> GetByIdAsync(long id);
        Task<ConfiguracaoLembreteDiarioDto> CreateAsync(CriarConfiguracaoLembreteDiarioRequest request);
        Task UpdateAsync(AtualizarConfiguracaoLembreteDiarioRequest request);
        Task DeleteAsync(long id);
        Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetActiveDailyRemindersAsync(); // Para uso pelo Hosted Service
    }
}
