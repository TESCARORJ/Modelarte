using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FuncaoService : IFuncaoService
    {
        private readonly IFuncaoRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuncaoService(IFuncaoRepository repo, ILogAuditoriaRepository logRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<FuncaoDto>> ObterTodasAsync()
        {
            var funcoes = await _repo.ObterTodasAsync();
            return _mapper.Map<IEnumerable<FuncaoDto>>(funcoes);
        }

        public async Task<FuncaoDto?> ObterPorIdAsync(long id)
        {
            var funcao = await _repo.GetByIdAsync(id);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task<FuncaoDto?> ObterPorNomeAsync(string nome)
        {
            var funcao = await _repo.ObterPorNomeAsync(nome);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task CriarAsync(FuncaoDto dto)
        {
            var entity = _mapper.Map<Funcao>(dto);
            _repo.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Criado",
                Descricao = $"Função '{entity.Nome}' criada",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(FuncaoDto dto)
        {
            var antiga = await _repo.GetByIdAsync(dto.Id);
            if (antiga == null) return;

            var nova = _mapper.Map<Funcao>(dto);

            _repo.Update(nova);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Atualizado",
                Descricao = $"Função '{nova.Nome}' atualizada",
                DadosAnteriores = JsonSerializer.Serialize(antiga),
                DadosAtuais = JsonSerializer.Serialize(nova)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;

            _repo.Remove(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcao),
                Acao = "Excluído",
                Descricao = $"Função '{entity.Nome}' removida",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });
        }
    }

}