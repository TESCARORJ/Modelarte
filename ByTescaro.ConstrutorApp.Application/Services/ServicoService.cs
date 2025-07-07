using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class ServicoService : IServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicoService(
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

        public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.ServicoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServicoDto>>(insumos);
        }

        public async Task<ServicoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<ServicoDto>(insumo);
        }

        public async Task CriarAsync(ServicoDto dto)
        {
            var entity = _mapper.Map<Servico>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            _unitOfWork.ServicoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, UsuarioLogado);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ServicoDto dto)
        {
            var entityAntigo = await _unitOfWork.ServicoRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Servico>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            _unitOfWork.ServicoRepository.Update(entityNovo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, UsuarioLogado);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.ServicoRepository.Remove(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, UsuarioLogado);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.ServicoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}