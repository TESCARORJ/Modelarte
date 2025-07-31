using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public OrcamentoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<OrcamentoDto> CriarAsync(OrcamentoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            // Calcular total com base nos itens
            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            var entidade = _mapper.Map<Orcamento>(dto);
            entidade.UsuarioCadastroId = usuarioLogadoId;
            entidade.DataHoraCadastro = DateTime.Now;

            // Preencher metadados dos itens
            foreach (var item in entidade.Itens)
            {
                item.UsuarioCadastroId = usuarioLogadoId;
                item.DataHoraCadastro = DateTime.Now;
            }

            _unitOfWork.OrcamentoRepository.Add(entidade);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);


            return _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<List<OrcamentoDto>> ObterTodosAsync()
        {
            var lista = await _unitOfWork.OrcamentoRepository.GetAllAsync();
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<List<OrcamentoDto>> ObterPorObraAsync(long obraId)
        {
            var lista = await _unitOfWork.OrcamentoRepository.GetByObraAsync(obraId);
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<OrcamentoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<OrcamentoDto?> ObterPorIdComItensAsync(long id)
        {
            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdComItensAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task AtualizarAsync(OrcamentoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;


            var orcamentoAntigoParaAuditoria = await _unitOfWork.OrcamentoRepository
                .GetByIdComItensNoTrackingAsync(dto.Id); 

            if (orcamentoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Orçamento com ID {dto.Id} não encontrado para auditoria.");
            }


            var orcamentoParaAtualizar = await _unitOfWork.OrcamentoRepository
                .GetByIdComItensAsync(dto.Id);

            if (orcamentoParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Orçamento com ID {dto.Id} não encontrado para atualização.");
            }

            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

          
            _mapper.Map(dto, orcamentoParaAtualizar);

            
            orcamentoParaAtualizar.UsuarioCadastroId = orcamentoAntigoParaAuditoria.UsuarioCadastroId;
            orcamentoParaAtualizar.DataHoraCadastro = orcamentoAntigoParaAuditoria.DataHoraCadastro;

          
            var itensParaRemover = orcamentoParaAtualizar.Itens
                .Where(existingItem => !dto.Itens.Any(dtoItem => dtoItem.Id == existingItem.Id && dtoItem.Id != 0))
                .ToList();

            foreach (var item in itensParaRemover)
            {
                orcamentoParaAtualizar.Itens.Remove(item);
                
            }

            foreach (var itemDto in dto.Itens)
            {
                var existingItem = orcamentoParaAtualizar.Itens.FirstOrDefault(i => i.Id == itemDto.Id && i.Id != 0);

                if (existingItem == null) 
                {
                    orcamentoParaAtualizar.Itens.Add(new OrcamentoItem
                    {
                        ServicoId = itemDto.ServicoId, 
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitario = itemDto.PrecoUnitario,
                        DataHoraCadastro = DateTime.Now,
                        UsuarioCadastroId = usuarioLogadoId,
                        OrcamentoObraId = orcamentoParaAtualizar.Id
                    });
                }
                else 
                {
                    existingItem.ServicoId = itemDto.ServicoId; 
                    existingItem.Quantidade = itemDto.Quantidade;
                    existingItem.PrecoUnitario = itemDto.PrecoUnitario;
    

                    
                }
            }

            await _unitOfWork.CommitAsync();
           
            await _auditoriaService.RegistrarAtualizacaoAsync(orcamentoAntigoParaAuditoria, orcamentoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdComItensAsync(id);
            if (entidade == null) return;

            _unitOfWork.OrcamentoRepository.Remove(entidade);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);
        }
    }
}
