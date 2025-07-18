using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface ILogAuditoriaService
    {
        Task<List<LogAuditoriaDto>> ObterTodosAsync();
    }

}
