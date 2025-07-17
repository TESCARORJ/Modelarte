using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoItemService : IOrcamentoItemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public OrcamentoItemService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<OrcamentoItemDto>> ObterPorOrcamentoIdAsync(long orcamentoId)
        {
            var itens = await _unitOfWork.OrcamentoItemRepository.GetByOrcamentoIdAsync(orcamentoId);
            return _mapper.Map<List<OrcamentoItemDto>>(itens);
        }


        public async Task CriarAsync(OrcamentoItemDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = _mapper.Map<OrcamentoItem>(dto);
            _unitOfWork.OrcamentoItemRepository.Update(entidade);

            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);
            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(OrcamentoItemDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'orcamentoItemAntigoParaAuditoria' NÃO será modificada e representa o estado original.
            var orcamentoItemAntigoParaAuditoria = await _unitOfWork.OrcamentoItemRepository.GetByIdNoTrackingAsync(dto.Id);

            if (orcamentoItemAntigoParaAuditoria is null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Item de Orçamento com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'orcamentoItemParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var orcamentoItemParaAtualizar = await _unitOfWork.OrcamentoItemRepository.GetByIdAsync(dto.Id);

            if (orcamentoItemParaAtualizar is null)
            {
                // Isso deve ser raro se 'orcamentoItemAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Item de Orçamento com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'orcamentoItemParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, orcamentoItemParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'orcamentoItemAntigoParaAuditoria'.
            // Exemplo:
            // orcamentoItemParaAtualizar.UsuarioCadastroId = orcamentoItemAntigoParaAuditoria.UsuarioCadastroId;
            // orcamentoItemParaAtualizar.DataHoraCadastro = orcamentoItemAntigoParaAuditoria.DataHoraCadastro;

            // O método .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.OrcamentoItemRepository.Update(orcamentoItemParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'orcamentoItemAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'orcamentoItemParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(orcamentoItemAntigoParaAuditoria, orcamentoItemParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.OrcamentoItemRepository.GetByIdAsync(id);
            if (entidade == null) return;

            _unitOfWork.OrcamentoItemRepository.Remove(entidade);
            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }
    }
}
