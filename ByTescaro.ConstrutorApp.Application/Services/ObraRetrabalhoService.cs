using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraRetrabalhoService : IObraRetrabalhoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraRetrabalhoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraRetrabalhoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraRetrabalhoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraRetrabalhoDto>>(list);
        }

        public async Task CriarAsync(ObraRetrabalhoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraRetrabalho>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraRetrabalhoRepository.Add(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);



        }

        public async Task AtualizarAsync(ObraRetrabalhoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

           
            var obraRetrabalhoAntigoParaAuditoria = await _unitOfWork.ObraRetrabalhoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraRetrabalhoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Retrabalho com ID {dto.Id} não encontrada para auditoria.");
            }

           
            var obraRetrabalhoParaAtualizar = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(dto.Id);

            if (obraRetrabalhoParaAtualizar == null)
            {
             
                throw new KeyNotFoundException($"Obra Retrabalho com ID {dto.Id} não encontrada para atualização.");
            }

            
            _mapper.Map(dto, obraRetrabalhoParaAtualizar);


            obraRetrabalhoParaAtualizar.UsuarioCadastroId = obraRetrabalhoAntigoParaAuditoria.UsuarioCadastroId;
            obraRetrabalhoParaAtualizar.DataHoraCadastro = obraRetrabalhoAntigoParaAuditoria.DataHoraCadastro;

          
            _unitOfWork.ObraRetrabalhoRepository.Update(obraRetrabalhoParaAtualizar);

            await _unitOfWork.CommitAsync();
          
            await _auditoriaService.RegistrarAtualizacaoAsync(obraRetrabalhoAntigoParaAuditoria, obraRetrabalhoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraRetrabalhoRepository.Remove(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        }
    }
}
