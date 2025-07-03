using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FornecedorService(
            IFornecedorRepository repo,
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

        public async Task<IEnumerable<FornecedorDto>> ObterTodosAsync()
        {
            var fornecedores = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<FornecedorDto>>(fornecedores);
        }

        public async Task<FornecedorDto?> ObterPorIdAsync(long id)
        {
            var fornecedor = await _repo.GetByIdAsync(id);
            return fornecedor == null ? null : _mapper.Map<FornecedorDto>(fornecedor);
        }

        public async Task CriarAsync(FornecedorDto dto)
        {
            var entity = _mapper.Map<Fornecedor>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _repo.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Fornecedor),
                Acao = "Criado",
                Descricao = $"Fornecedor '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(FornecedorDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Fornecedor>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _repo.Update(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Fornecedor),
                Acao = "Atualizado",
                Descricao = $"Fornecedor '{entityNovo.Nome}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
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
                Entidade = nameof(Fornecedor),
                Acao = "Excluído",
                Descricao = $"Fornecedor '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });
        }
    }

}