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
    public class ObraInsumoListaService : IObraInsumoListaService
    {
        private readonly IObraInsumoListaRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ObraInsumoListaService(IObraInsumoListaRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraInsumoListaDto>> ObterPorObraIdAsync(long obraId)
        {
            var listas = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraInsumoListaDto>>(listas);
        }

        public async Task<ObraInsumoListaDto?> ObterPorIdAsync(long id)
        {
            var entity = await _repo.GetByIdWithItensAsync(id);
            return entity == null ? null : _mapper.Map<ObraInsumoListaDto>(entity);
        }

        public async Task<ObraInsumoListaDto> CriarAsync(ObraInsumoListaDto dto)
        {
            var entity = _mapper.Map<ObraInsumoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            await _repo.AddAsync(entity);

            // Recarrega com os relacionamentos (Responsável, Insumo)
            var atualizado = await _repo.GetByIdWithItensAsync(entity.Id);

            return _mapper.Map<ObraInsumoListaDto>(atualizado);
        }



        public async Task AtualizarAsync(ObraInsumoListaDto dto)
        {
            var entity = await _repo.GetByIdWithItensAsync(dto.Id);
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

            await _repo.UpdateAsync(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.RemoveAsync(entity);
        }
    }
}
