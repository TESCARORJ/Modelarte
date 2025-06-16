using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class InsumoService : IInsumoService
    {
        private readonly IInsumoRepository _repo;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InsumoService(
            IInsumoRepository repo,
            IAuditoriaService auditoriaService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _auditoriaService = auditoriaService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<InsumoDto>> ObterTodosAsync()
        {
            var insumos = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<InsumoDto>>(insumos);
        }

        public async Task<InsumoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _repo.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<InsumoDto>(insumo);
        }

        public async Task CriarAsync(InsumoDto dto)
        {
            var entity = _mapper.Map<Insumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            await _repo.AddAsync(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, UsuarioLogado);
        }

        public async Task AtualizarAsync(InsumoDto dto)
        {
            var entityAntigo = await _repo.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Insumo>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            await _repo.UpdateAsync(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, UsuarioLogado);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;

            await _repo.RemoveAsync(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, UsuarioLogado);
        }
    }

}