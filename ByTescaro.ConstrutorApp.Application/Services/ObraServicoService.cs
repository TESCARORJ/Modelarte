using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoService : IObraServicoService
    {
        private readonly IObraServicoRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ObraServicoService(IObraServicoRepository repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraServicoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _repo.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraServicoDto>>(itens);
        }

        public async Task CriarAsync(ObraServicoDto dto)
        {
            dto.DataHoraCadastro = DateTime.Now;
            dto.UsuarioCadastro = UsuarioLogado;
            var entity = _mapper.Map<ObraServico>(dto);
            await _repo.AddAsync(entity);
        }

        public async Task AtualizarAsync(ObraServicoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.RemoveAsync(entity);
        }

        public async Task<List<ServicoDto>> ObterServicosDisponiveisAsync(long obraId)
        {
            var entidades = await _repo.GetServicosDisponiveisAsync(obraId);
            return _mapper.Map<List<ServicoDto>>(entidades);
        }

    }
}
