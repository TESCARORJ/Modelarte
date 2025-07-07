using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FuncaoService : IFuncaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuncaoService(ILogAuditoriaRepository logRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<FuncaoDto>> ObterTodasAsync()
        {
            var funcoes = await _unitOfWork.FuncaoRepository.ObterTodasAsync();
            return _mapper.Map<IEnumerable<FuncaoDto>>(funcoes);
        }

        public async Task<FuncaoDto?> ObterPorIdAsync(long id)
        {
            var funcao = await _unitOfWork.FuncaoRepository.GetByIdAsync(id);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task<FuncaoDto?> ObterPorNomeAsync(string nome)
        {
            var funcao = await _unitOfWork.FuncaoRepository.ObterPorNomeAsync(nome);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task CriarAsync(FuncaoDto dto)
        {
            var entity = _mapper.Map<Funcao>(dto);
            _unitOfWork.FuncaoRepository.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Criado",
                Descricao = $"Função '{entity.Nome}' criada",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(FuncaoDto dto)
        {
            var antiga = await _unitOfWork.FuncaoRepository.GetByIdAsync(dto.Id);
            if (antiga == null) return;

            var nova = _mapper.Map<Funcao>(dto);

            _unitOfWork.FuncaoRepository.Update(nova);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Atualizado",
                Descricao = $"Função '{nova.Nome}' atualizada",
                DadosAnteriores = JsonSerializer.Serialize(antiga),
                DadosAtuais = JsonSerializer.Serialize(nova)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.FuncaoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.FuncaoRepository.Remove(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Excluído",
                Descricao = $"Função '{entity.Nome}' removida",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return false; // Nome vazio não é considerado duplicado
            }
            return await _unitOfWork.FuncaoRepository.ExistsAsync(f =>
                f.Nome == nome && (ignoreId == null || f.Id != ignoreId.Value));
        }
    }

}