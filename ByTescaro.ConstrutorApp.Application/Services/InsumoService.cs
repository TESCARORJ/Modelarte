using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class InsumoService : IInsumoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public InsumoService(
            IAuditoriaService auditoriaService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUsuarioLogadoService usuarioLogadoService)
        {
            _auditoriaService = auditoriaService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<IEnumerable<InsumoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.InsumoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InsumoDto>>(insumos);
        }

        public async Task<InsumoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<InsumoDto>(insumo);
        }

        public async Task CriarAsync(InsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Insumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.InsumoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

        }

        public async Task AtualizarAsync(InsumoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var insumoParaAtualizar = await _unitOfWork.InsumoRepository.GetByIdAsync(dto.Id);

            if (insumoParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Insumo com ID {dto.Id} não encontrado para atualização.");
            }

            var insumoAntigoParaAuditoria = _mapper.Map<Insumo>(insumoParaAtualizar);

            _mapper.Map(dto, insumoParaAtualizar);

            _unitOfWork.InsumoRepository.Update(insumoParaAtualizar);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(insumoAntigoParaAuditoria, insumoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.InsumoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
        }


        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.InsumoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}