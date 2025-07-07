using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoListaService : IObraInsumoListaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ObraInsumoListaService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

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
            var entity = _mapper.Map<ObraInsumoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _unitOfWork.ObraInsumoListaRepository.Add(entity);

            await _unitOfWork.CommitAsync();
            
            var atualizado = await _unitOfWork.ObraInsumoListaRepository.GetByIdWithItensAsync(entity.Id);

            return _mapper.Map<ObraInsumoListaDto>(atualizado);
        }



        public async Task AtualizarAsync(ObraInsumoListaDto dto)
        {
            var entity = await _unitOfWork.ObraInsumoListaRepository.GetByIdWithItensAsync(dto.Id);
            if (entity == null) return;

            // Atualiza os dados principais da lista
            entity.Data = dto.Data.ToDateTime(TimeOnly.MinValue);
            entity.ResponsavelId = dto.ResponsavelId;

            // Atualiza os itens (remoção e adição simples)
            entity.Itens.Clear();
            foreach (var itemDto in dto.Itens)
            {

                entity.Itens.Add(new ObraInsumo
                {
                    InsumoId = itemDto.InsumoId,
                    Quantidade = itemDto.Quantidade,
                    DataHoraCadastro = DateTime.Now,
                    UsuarioCadastro = UsuarioLogado
                });
            }

            _unitOfWork.ObraInsumoListaRepository.Update(entity);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraInsumoListaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraInsumoListaRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
