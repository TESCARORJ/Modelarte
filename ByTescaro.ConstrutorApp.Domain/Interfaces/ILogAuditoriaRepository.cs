using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface ILogAuditoriaRepository
    {
        Task RegistrarAsync(LogAuditoria log);
        Task<List<LogAuditoria>> ObterTodosAsync();
    }

}
