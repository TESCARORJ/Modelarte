using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class LogAuditoriaService : ILogAuditoriaService
    {
        private readonly ILogAuditoriaRepository _repo;

        public LogAuditoriaService(ILogAuditoriaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LogAuditoria>> ObterTodosAsync()
        {
            return await _repo.ObterTodosAsync();
        }
    }

}
