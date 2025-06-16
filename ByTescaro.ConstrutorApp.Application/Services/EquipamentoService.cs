using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class EquipamentoService : IEquipamentoService
    {
        private readonly IEquipamentoRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EquipamentoService(
            IEquipamentoRepository repo,
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<EquipamentoDto>> ObterTodosAsync()
        {
            var equipamentos = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<EquipamentoDto>>(equipamentos);
        }

        public async Task<EquipamentoDto?> ObterPorIdAsync(long id)
        {
            var equipamento = await _repo.GetByIdAsync(id);
            return equipamento == null ? null : _mapper.Map<EquipamentoDto>(equipamento);
        }

        public async Task CriarAsync(EquipamentoDto dto)
        {
            var entity = _mapper.Map<Equipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            entity.Status = Domain.Enums.StatusEquipamento.Disponivel;

            await _repo.AddAsync(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Equipamento),
                Acao = "Criado",
                Descricao = $"Equipamento '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(EquipamentoDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Equipamento>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            await _repo.UpdateAsync(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Equipamento),
                Acao = "Atualizado",
                Descricao = $"Equipamento '{entityNovo.Nome}' atualizado",
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
                Entidade = nameof(Equipamento),
                Acao = "Excluído",
                Descricao = $"Equipamento '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });
        }

        public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        {
            return await _repo.ObterResumoAlocacaoAsync();
        }
    }

}