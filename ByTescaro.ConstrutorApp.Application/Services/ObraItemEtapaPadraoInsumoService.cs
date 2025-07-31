using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoInsumoService : IObraItemEtapaPadraoInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraItemEtapaPadraoInsumoService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraItemEtapaPadraoInsumoDto>> ObterPorItemPadraoIdAsync(long itemPadraoId)
        {
            var lista = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByItemPadraoIdAsync(itemPadraoId);
            return _mapper.Map<List<ObraItemEtapaPadraoInsumoDto>>(lista);
        }

        public async Task CriarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = _mapper.Map<ObraItemEtapaPadraoInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Add(entidade);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);

        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            
            var obraItemEtapaPadraoInsumoAntigoParaAuditoria = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraItemEtapaPadraoInsumoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Item Etapa Padrão Insumo com ID {dto.Id} não encontrado para auditoria.");
            }

           
            var obraItemEtapaPadraoInsumoParaAtualizar = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(dto.Id);

            if (obraItemEtapaPadraoInsumoParaAtualizar == null)
            {
              
                throw new KeyNotFoundException($"Obra Item Etapa Padrão Insumo com ID {dto.Id} não encontrado para atualização.");
            }

         
            _mapper.Map(dto, obraItemEtapaPadraoInsumoParaAtualizar);

            
            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarAtualizacaoAsync(obraItemEtapaPadraoInsumoAntigoParaAuditoria, obraItemEtapaPadraoInsumoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Remove(entidade);
            
            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);


        }
    }
}
