using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class EquipamentoService : IEquipamentoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EquipamentoService(            
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {            
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<EquipamentoDto>> ObterTodosAsync()
        {
            var equipamentos = await _unitOfWork.EquipamentoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EquipamentoDto>>(equipamentos);
        }

        public async Task<EquipamentoDto?> ObterPorIdAsync(long id)
        {
            var equipamento = await _unitOfWork.EquipamentoRepository.GetByIdAsync(id);
            return equipamento == null ? null : _mapper.Map<EquipamentoDto>(equipamento);
        }

        public async Task CriarAsync(EquipamentoDto dto)
        {
            var entity = _mapper.Map<Equipamento>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            entity.Status = Domain.Enums.StatusEquipamento.Disponivel;

            _unitOfWork.EquipamentoRepository.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Equipamento),
                Acao = "Criado",
                Descricao = $"Equipamento '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(EquipamentoDto dto)
        {
            var entityAntigo = await _unitOfWork.EquipamentoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Equipamento>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.EquipamentoRepository.Update(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Equipamento),
                Acao = "Atualizado",
                Descricao = $"Equipamento '{entityNovo.Nome}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.EquipamentoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.EquipamentoRepository.Remove(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Equipamento),
                Acao = "Excluído",
                Descricao = $"Equipamento '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();
        }

        //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        //{
        //    return await _repo.ObterResumoAlocacaoAsync();
        //}

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.EquipamentoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }

        public async Task<bool> PatrimonioExistsAsync(string patrimonio, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(patrimonio)) return false;
            return await _unitOfWork.EquipamentoRepository.ExistsAsync(e =>
                e.Patrimonio == patrimonio && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}