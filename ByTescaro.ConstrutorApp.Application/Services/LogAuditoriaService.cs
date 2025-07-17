    using ByTescaro.ConstrutorApp.Application.DTOs;
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

            public async Task<List<LogAuditoriaDTO>> ObterTodosAsync()
            {
                var logs = await _repo.ObterTodosAsync();

                // Mapeamento de LogAuditoria para LogAuditoriaDTO
                var dtos = logs.Select(log => new LogAuditoriaDTO
                {
                    Id = log.Id,
                    UsuarioId = log.UsuarioId,
                    UsuarioNome = log.UsuarioNome,
                    Entidade = log.Entidade,
                    TipoLogAuditoria = log.TipoLogAuditoria,
                    Descricao = log.Descricao,
                    DataHora = log.DataHora,
                    DadosAnteriores = log.DadosAnteriores,
                    DadosAtuais = log.DadosAtuais,
                    IdEntidade = log.IdEntidade
                }).ToList();

                return dtos;
            }
        }

    }
