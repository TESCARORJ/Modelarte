using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoInsumoService : IObraItemEtapaPadraoInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public ObraItemEtapaPadraoInsumoService(IMapper mapper, IHttpContextAccessor http, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _http = http;
            _unitOfWork = unitOfWork;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraItemEtapaPadraoInsumoDto>> ObterPorItemPadraoIdAsync(long itemPadraoId)
        {
            var lista = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByItemPadraoIdAsync(itemPadraoId);
            return _mapper.Map<List<ObraItemEtapaPadraoInsumoDto>>(lista);
        }

        public async Task CriarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var entidade = _mapper.Map<ObraItemEtapaPadraoInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastro = Usuario;
            _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Add(entidade);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var entidade = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(dto.Id);
            if (entidade == null) return;

            _mapper.Map(dto, entidade);
            _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Update(entidade);
            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var entidade = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Remove(entidade);
            await _unitOfWork.CommitAsync();

        }
    }
}
