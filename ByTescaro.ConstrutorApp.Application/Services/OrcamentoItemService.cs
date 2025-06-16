using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoItemService : IOrcamentoItemService
    {
        private readonly IOrcamentoItemRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;

        public OrcamentoItemService(IOrcamentoItemRepository repo, ILogAuditoriaRepository logRepo, IMapper mapper)
        {
            _repo = repo;
            _logRepo = logRepo;
            _mapper = mapper;
        }

        public async Task<List<OrcamentoItemDto>> ObterPorOrcamentoIdAsync(long orcamentoId)
        {
            var itens = await _repo.GetByOrcamentoIdAsync(orcamentoId);
            return _mapper.Map<List<OrcamentoItemDto>>(itens);
        }


        public async Task CriarAsync(OrcamentoItemDto dto)
        {
            var entidade = _mapper.Map<OrcamentoItem>(dto);
            await _repo.AddAsync(entidade);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = "Sistema", // ou use IHttpContextAccessor
                Entidade = nameof(OrcamentoItem),
                Acao = "Criado",
                Descricao = $"Item criado no orçamento {entidade.OrcamentoObraId}",
                DadosAtuais = JsonSerializer.Serialize(entidade)
            });
        }

        public async Task AtualizarAsync(OrcamentoItemDto dto)
        {
            var atual = await _repo.GetByIdAsync(dto.Id);
            if (atual is null) return;

            var novo = _mapper.Map<OrcamentoItem>(dto);
            await _repo.UpdateAsync(novo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = "Sistema",
                Entidade = nameof(OrcamentoItem),
                Acao = "Atualizado",
                Descricao = $"Item atualizado no orçamento {novo.OrcamentoObraId}",
                DadosAnteriores = JsonSerializer.Serialize(atual),
                DadosAtuais = JsonSerializer.Serialize(novo)
            });
        }

        public async Task RemoverAsync(long id)
        {
            var entidade = await _repo.GetByIdAsync(id);
            if (entidade == null) return;

            await _repo.RemoveAsync(entidade);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = "Sistema",
                Entidade = nameof(OrcamentoItem),
                Acao = "Removido",
                Descricao = $"Item removido do orçamento {entidade.OrcamentoObraId}",
                DadosAnteriores = JsonSerializer.Serialize(entidade)
            });
        }
    }
}
