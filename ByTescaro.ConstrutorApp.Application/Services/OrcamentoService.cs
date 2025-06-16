using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IOrcamentoRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public OrcamentoService(
            IOrcamentoRepository repo,
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor http)
        {
            _repo = repo;
            _logRepo = logRepo;
            _mapper = mapper;
            _http = http;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<OrcamentoDto> CriarAsync(OrcamentoDto dto)
        {
            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            // Calcular total com base nos itens
            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            var entidade = _mapper.Map<Orcamento>(dto);
            entidade.UsuarioCadastro = Usuario;
            entidade.DataHoraCadastro = DateTime.Now;

            // Preencher metadados dos itens
            foreach (var item in entidade.Itens)
            {
                item.UsuarioCadastro = Usuario;
                item.DataHoraCadastro = DateTime.Now;
            }

            await _repo.AddAsync(entidade);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = Usuario,
                Entidade = nameof(Orcamento),
                Acao = "Criado",
                Descricao = $"Orçamento criado para Obra {entidade.ObraId} com {entidade.Itens.Count} item(ns)",
                DadosAtuais = JsonSerializer.Serialize(entidade)
            });

            return _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<List<OrcamentoDto>> ObterTodosAsync()
        {
            var lista = await _repo.GetAllAsync();
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<List<OrcamentoDto>> ObterPorObraAsync(long obraId)
        {
            var lista = await _repo.GetByObraAsync(obraId);
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<OrcamentoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _repo.GetByIdAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<OrcamentoDto?> ObterPorIdComItensAsync(long id)
        {
            var entidade = await _repo.GetByIdComItensAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task AtualizarAsync(OrcamentoDto dto)
        {
            var original = await _repo.GetByIdComItensAsync(dto.Id);
            if (original == null) return;

            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            var entidade = _mapper.Map<Orcamento>(dto);
            entidade.UsuarioCadastro = original.UsuarioCadastro;
            entidade.DataHoraCadastro = original.DataHoraCadastro;

            foreach (var item in entidade.Itens)
            {
                item.UsuarioCadastro = Usuario;
                item.DataHoraCadastro = DateTime.Now;
            }

            await _repo.UpdateAsync(entidade);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = Usuario,
                Entidade = nameof(Orcamento),
                Acao = "Atualizado",
                Descricao = $"Orçamento {entidade.Id} atualizado",
                DadosAnteriores = JsonSerializer.Serialize(original),
                DadosAtuais = JsonSerializer.Serialize(entidade)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entidade = await _repo.GetByIdComItensAsync(id);
            if (entidade == null) return;

            await _repo.RemoveAsync(entidade);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = Usuario,
                Entidade = nameof(Orcamento),
                Acao = "Removido",
                Descricao = $"Orçamento {entidade.Id} removido",
                DadosAnteriores = JsonSerializer.Serialize(entidade)
            });
        }
    }
}
