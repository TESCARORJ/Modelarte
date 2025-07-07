using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class InsumoService : IInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InsumoService(
            IAuditoriaService auditoriaService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _auditoriaService = auditoriaService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<InsumoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.InsumoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InsumoDto>>(insumos);
        }

        public async Task<InsumoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<InsumoDto>(insumo);
        }

        public async Task CriarAsync(InsumoDto dto)
        {
            var entity = _mapper.Map<Insumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _unitOfWork.InsumoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, UsuarioLogado);

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(InsumoDto dto)
        {
            var entityAntigo = await _unitOfWork.InsumoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Insumo>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.InsumoRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, UsuarioLogado);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.InsumoRepository.Remove(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, UsuarioLogado);
            await _unitOfWork.CommitAsync();
        }


        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.InsumoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}