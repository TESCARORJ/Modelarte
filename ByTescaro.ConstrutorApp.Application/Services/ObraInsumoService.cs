using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoService : IObraInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraInsumoService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraInsumoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraInsumoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraInsumoDto>>(itens);
        }

        public async Task CriarAsync(ObraInsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraInsumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraInsumoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ObraInsumoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;


            var obraInsumoAntigoParaAuditoria = await _unitOfWork.ObraInsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraInsumoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Insumo com ID {dto.Id} não encontrado para auditoria.");
            }

            var obraInsumoParaAtualizar = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(dto.Id);

            if (obraInsumoParaAtualizar == null)
            {
 
                throw new KeyNotFoundException($"Obra Insumo com ID {dto.Id} não encontrado para atualização.");
            }


            _mapper.Map(dto, obraInsumoParaAtualizar);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarAtualizacaoAsync(obraInsumoAntigoParaAuditoria, obraInsumoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraInsumoRepository.Remove(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        }

        public async Task<List<InsumoDto>> ObterInsumosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosDisponiveisAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

        public async Task<List<InsumoDto>> ObterInsumosPorPadraoObraAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosPorPadraoObraAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

        public async Task<List<InsumoDto>> ObterInsumosAsync()
        {
            var entidades = await _unitOfWork.InsumoRepository.FindAsync(x => x.Ativo == true);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

    }
}
