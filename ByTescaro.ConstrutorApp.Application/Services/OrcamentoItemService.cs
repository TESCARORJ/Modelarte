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

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);

        }

        public async Task AtualizarAsync(OrcamentoItemDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            
            var orcamentoItemAntigoParaAuditoria = await _unitOfWork.OrcamentoItemRepository.GetByIdNoTrackingAsync(dto.Id);

            if (orcamentoItemAntigoParaAuditoria is null)
            {
                throw new KeyNotFoundException($"Item de Orçamento com ID {dto.Id} não encontrado para auditoria.");
            }


            var orcamentoItemParaAtualizar = await _unitOfWork.OrcamentoItemRepository.GetByIdAsync(dto.Id);

            if (orcamentoItemParaAtualizar is null)
            {

                throw new KeyNotFoundException($"Item de Orçamento com ID {dto.Id} não encontrado para atualização.");
            }


            _mapper.Map(dto, orcamentoItemParaAtualizar);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarAtualizacaoAsync(orcamentoItemAntigoParaAuditoria, orcamentoItemParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.OrcamentoItemRepository.GetByIdAsync(id);
            if (entidade == null) return;

            _unitOfWork.OrcamentoItemRepository.Remove(entidade);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);
        }
    }
}
