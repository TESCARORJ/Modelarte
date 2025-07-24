using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraPendenciaService : IObraPendenciaService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraPendenciaService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraPendenciaDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraPendenciaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraPendenciaDto>>(list);
        }

        public async Task CriarAsync(ObraPendenciaDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraPendencia>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraPendenciaRepository.Add(entity);



            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ObraPendenciaDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var obraPendenciaAntigaParaAuditoria = await _unitOfWork.ObraPendenciaRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraPendenciaAntigaParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Pendência com ID {dto.Id} não encontrada para auditoria.");
            }


            var obraPendenciaParaAtualizar = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(dto.Id);

            if (obraPendenciaParaAtualizar == null)
            {

                throw new KeyNotFoundException($"Obra Pendência com ID {dto.Id} não encontrada para atualização.");
            }


            _mapper.Map(dto, obraPendenciaParaAtualizar);


            obraPendenciaParaAtualizar.UsuarioCadastroId = obraPendenciaAntigaParaAuditoria.UsuarioCadastroId;
            obraPendenciaParaAtualizar.DataHoraCadastro = obraPendenciaAntigaParaAuditoria.DataHoraCadastro;


            await _auditoriaService.RegistrarAtualizacaoAsync(obraPendenciaAntigaParaAuditoria, obraPendenciaParaAtualizar, usuarioLogadoId);

            _unitOfWork.ObraPendenciaRepository.Update(obraPendenciaParaAtualizar);

            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraPendenciaRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }
    }
}
