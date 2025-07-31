using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoListaService : IObraInsumoListaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraInsumoListaService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraInsumoListaDto>> ObterPorObraIdAsync(long obraId)
        {
            var listas = await _unitOfWork.ObraInsumoListaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraInsumoListaDto>>(listas);
        }

        public async Task<ObraInsumoListaDto?> ObterPorIdAsync(long id)
        {
            var entity = await _unitOfWork.ObraInsumoListaRepository.GetByIdWithItensAsync(id);
            return entity == null ? null : _mapper.Map<ObraInsumoListaDto>(entity);
        }

        public async Task<ObraInsumoListaDto> CriarAsync(ObraInsumoListaDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraInsumoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraInsumoListaRepository.Add(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            
            var atualizado = await _unitOfWork.ObraInsumoListaRepository.GetByIdWithItensAsync(entity.Id);

            return _mapper.Map<ObraInsumoListaDto>(atualizado);
        }

        public async Task AtualizarAsync(ObraInsumoListaDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var listaAntigaParaAuditoria = await _unitOfWork.ObraInsumoListaRepository
                .GetByIdWithItensNoTrackingAsync(dto.Id); 

            if (listaAntigaParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Lista de Insumos da Obra com ID {dto.Id} não encontrada para auditoria.");
            }

            var listaParaAtualizar = await _unitOfWork.ObraInsumoListaRepository
                .GetByIdWithItensAsync(dto.Id);

            if (listaParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Lista de Insumos da Obra com ID {dto.Id} não encontrada para atualização.");
            }

            listaParaAtualizar.Data = dto.Data.ToDateTime(TimeOnly.MinValue);
            listaParaAtualizar.ResponsavelId = dto.ResponsavelId;


            var itensParaRemover = listaParaAtualizar.Itens
                .Where(existingItem => !dto.Itens.Any(dtoItem => dtoItem.Id == existingItem.Id && dtoItem.Id != 0))
                .ToList(); 
            foreach (var item in itensParaRemover)
            {
                listaParaAtualizar.Itens.Remove(item);
                _unitOfWork.ObraInsumoRepository.Remove(item);
            }

            foreach (var itemDto in dto.Itens)
            {
                var existingItem = listaParaAtualizar.Itens.FirstOrDefault(i => i.Id == itemDto.Id && i.Id != 0);

                if (existingItem == null) 
                {
                    listaParaAtualizar.Itens.Add(new ObraInsumo
                    {
                        InsumoId = itemDto.InsumoId,
                        Quantidade = itemDto.Quantidade,
                        DataHoraCadastro = DateTime.Now,
                        UsuarioCadastroId = usuarioLogadoId,
                        ObraInsumoListaId = listaParaAtualizar.Id
                    });
                }
                else 
                {
                    existingItem.InsumoId = itemDto.InsumoId;
                    existingItem.Quantidade = itemDto.Quantidade;
                    existingItem.IsRecebido = itemDto.IsRecebido; 
                    existingItem.DataRecebimento = itemDto.DataRecebimento; 



                    // Não é necessário chamar _unitOfWork.ObraInsumoRepository.Update(existingItem);
                    // O EF Core já está rastreando existingItem e detectará as mudanças.
                }
            }
                       
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(listaAntigaParaAuditoria, listaParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraInsumoListaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraInsumoListaRepository.Remove(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        }
    }
}
