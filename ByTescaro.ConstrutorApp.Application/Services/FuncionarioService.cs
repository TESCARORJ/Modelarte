using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FuncionarioService : IFuncionarioService
    {
        private readonly IFuncionarioRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuncionarioService(
            IFuncionarioRepository repo,
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

        public async Task<IEnumerable<FuncionarioDto>> ObterTodosAsync()
        {
            var funcionarios = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios);
        }

        public async Task<FuncionarioDto?> ObterPorIdAsync(long id)
        {
            var funcionario = await _repo.GetByIdAsync(id);
            return funcionario == null ? null : _mapper.Map<FuncionarioDto>(funcionario);
        }

        public async Task CriarAsync(FuncionarioDto dto)
        {
            var entity = _mapper.Map<Funcionario>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _repo.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcionario),
                Acao = "Criado",
                Descricao = $"Funcionario '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });
        }

        public async Task AtualizarAsync(FuncionarioDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Funcionario>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _repo.Update(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcionario),
                Acao = "Atualizado",
                Descricao = $"Funcionario '{entityNovo.Nome}' atualizado",
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
                Entidade = nameof(Funcionario),
                Acao = "Excluído",
                Descricao = $"Funcionario '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });
        }

        //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
        //{
        //    return await _repo.ObterResumoAlocacaoAsync();
        //}
    }

}