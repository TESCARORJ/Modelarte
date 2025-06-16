using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteService(
            IClienteRepository repo,
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
        {
            var clientes = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto?> ObterPorIdAsync(long id)
        {
            var cliente = await _repo.GetByIdAsync(id);
            return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
        }

        public async Task CriarAsync(ClienteDto dto)
        {
            var entity = _mapper.Map<Cliente>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            await _repo.AddAsync(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Cliente '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(ClienteDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Cliente>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            await _repo.UpdateAsync(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Atualizado",
                Descricao = $"Cliente '{entityNovo.Nome}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;

            await _repo.RemoveAsync(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Excluído",
                Descricao = $"Cliente '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });
        }
    }

}