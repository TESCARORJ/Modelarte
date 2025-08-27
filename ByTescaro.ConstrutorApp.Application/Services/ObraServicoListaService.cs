using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoListaService : IObraServicoListaService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraServicoListaService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraServicoListaDto>> ObterPorObraIdAsync(long obraId)
        {
            var listas = await _unitOfWork.ObraServicoListaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraServicoListaDto>>(listas);
        }

        public async Task<ObraServicoListaDto?> ObterPorIdAsync(long id)
        {
            var entity = await _unitOfWork.ObraServicoListaRepository.GetByIdWithItensAsync(id);
            return entity == null ? null : _mapper.Map<ObraServicoListaDto>(entity);
        }

        public async Task<ObraServicoListaDto> CriarAsync(ObraServicoListaDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraServicoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;


            foreach (var item in entity.Itens)
            {
                item.ObraServicoListaId = 0;
                item.ObraId = dto.ObraId;
                item.UsuarioCadastroId = usuarioLogadoId;
                item.DataHoraCadastro = DateTime.Now;
            }

            _unitOfWork.ObraServicoListaRepository.Add(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            var entidadeSalva = await _unitOfWork.ObraServicoListaRepository.GetByIdWithItensAsync(entity.Id);

            return _mapper.Map<ObraServicoListaDto>(entidadeSalva);
        }

        public async Task AtualizarAsync(ObraServicoListaDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var listaAntigaParaAuditoria = await _unitOfWork.ObraServicoListaRepository
                .GetByIdWithItensNoTrackingAsync(dto.Id); 

            if (listaAntigaParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Lista de Serviços da Obra com ID {dto.Id} não encontrada para auditoria.");
            }

            var listaParaAtualizar = await _unitOfWork.ObraServicoListaRepository
                .GetByIdWithItensAsync(dto.Id);

            if (listaParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Lista de Serviços da Obra com ID {dto.Id} não encontrada para atualização.");
            }

            listaParaAtualizar.Data = dto.Data.ToDateTime(TimeOnly.MinValue);
            listaParaAtualizar.ResponsavelId = dto.ResponsavelId;


            var itensParaRemover = listaParaAtualizar.Itens
                .Where(existingItem => !dto.Itens.Any(dtoItem => dtoItem.Id == existingItem.Id && dtoItem.Id != 0))
                .ToList();

            foreach (var item in itensParaRemover)
            {
                listaParaAtualizar.Itens.Remove(item);
               
            }

            foreach (var itemDto in dto.Itens)
            {
                var existingItem = listaParaAtualizar.Itens.FirstOrDefault(i => i.Id == itemDto.Id && i.Id != 0);

                if (existingItem == null)
                {
                    listaParaAtualizar.Itens.Add(new ObraServico
                    {
                        ServicoId = itemDto.ServicoId,
                        Quantidade = itemDto.Quantidade,
                        DataHoraCadastro = DateTime.Now,
                        UsuarioCadastroId = usuarioLogadoId,
                        ObraServicoListaId = listaParaAtualizar.Id, 
                        ObraId = itemDto.ObraId
                       
                    });
                }
                else 
                {
                    existingItem.ServicoId = itemDto.ServicoId;
                    existingItem.Quantidade = itemDto.Quantidade;
            

                }
            }



            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(listaAntigaParaAuditoria, listaParaAtualizar, usuarioLogadoId);
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraServicoListaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraServicoListaRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }
    }
}
