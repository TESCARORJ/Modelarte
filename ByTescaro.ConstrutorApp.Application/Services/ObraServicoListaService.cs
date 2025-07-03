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
        private readonly IObraServicoListaRepository _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ObraServicoListaService(IObraServicoListaRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraServicoListaDto>> ObterPorObraIdAsync(long obraId)
        {
            var listas = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraServicoListaDto>>(listas);
        }

        public async Task<ObraServicoListaDto?> ObterPorIdAsync(long id)
        {
            var entity = await _repo.GetByIdWithItensAsync(id);
            return entity == null ? null : _mapper.Map<ObraServicoListaDto>(entity);
        }

        public async Task<ObraServicoListaDto> CriarAsync(ObraServicoListaDto dto)
        {
            var entity = _mapper.Map<ObraServicoLista>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _repo.Add(entity);

            // Recarrega com os relacionamentos (Responsável, Servico)
            var atualizado = await _repo.GetByIdWithItensAsync(entity.Id);

            return _mapper.Map<ObraServicoListaDto>(atualizado);
        }

        public async Task AtualizarAsync(ObraServicoListaDto dto)
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

                entity.Itens.Add(new ObraServico
                {
                    ServicoId = itemDto.ServicoId,
                    Quantidade = itemDto.Quantidade,
                    DataHoraCadastro = DateTime.Now,
                    UsuarioCadastro = UsuarioLogado
                });
            }

            _repo.Update(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                _repo.Remove(entity);
        }
    }
}
