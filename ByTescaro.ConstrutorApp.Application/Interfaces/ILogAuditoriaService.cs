using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface ILogAuditoriaService
    {
        Task<List<LogAuditoria>> ObterTodosAsync();
    }

}
