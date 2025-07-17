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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entityAntigo = await _unitOfWork.OrcamentoItemRepository.GetByIdAsync(dto.Id);
            if (entityAntigo is null) return;

            var entityNovo = _mapper.Map<OrcamentoItem>(dto);
            _unitOfWork.OrcamentoItemRepository.Update(entityNovo);

            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);
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
