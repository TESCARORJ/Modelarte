using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoService : IObraInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ObraInsumoService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraInsumoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraInsumoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraInsumoDto>>(itens);
        }

        public async Task CriarAsync(ObraInsumoDto dto)
        {
            var entity = _mapper.Map<ObraInsumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;
            _unitOfWork.ObraInsumoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraInsumoDto dto)
        {
            var entity = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            _unitOfWork.ObraInsumoRepository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraInsumoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<InsumoDto>> ObterInsumosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosDisponiveisAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

        public async Task<List<InsumoDto>> ObterInsumosPorPadraoObraAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosPorPadraoObraAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

    }
}
