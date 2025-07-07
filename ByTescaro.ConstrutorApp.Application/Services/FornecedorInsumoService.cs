using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorInsumoService : IFornecedorInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public FornecedorInsumoService(IMapper mapper, IHttpContextAccessor http, IUnitOfWork unitOfWork)
        {            
            _mapper = mapper;
            _http = http;
            _unitOfWork = unitOfWork;
        }

        private string Usuario => _http.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<FornecedorInsumoDto>> ObterTodosAsync()
        {
            var lista = await _unitOfWork.FornecedorInsumoRepository.GetAllAsync();
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<FornecedorInsumoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _unitOfWork.FornecedorInsumoRepository.GetByIdAsync(id);
            return entidade == null ? null : _mapper.Map<FornecedorInsumoDto>(entidade);
        }

        public async Task<List<FornecedorInsumoDto>> ObterPorFornecedorAsync(long fornecedorId)
        {
            var lista = await _unitOfWork.FornecedorInsumoRepository.ObterPorFornecedorIdAsync(fornecedorId);
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<List<FornecedorInsumoDto>> ObterPorInsumoAsync(long insumoId)
        {
            var lista = await _unitOfWork.FornecedorInsumoRepository.ObterPorInsumoIdAsync(insumoId);
            return _mapper.Map<List<FornecedorInsumoDto>>(lista);
        }

        public async Task<long> CriarAsync(FornecedorInsumoDto dto)
        {
            var entidade = _mapper.Map<FornecedorInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastro = Usuario;
            _unitOfWork.FornecedorInsumoRepository.Add(entidade);
            await _unitOfWork.CommitAsync();
            return entidade.Id;
        }

        public async Task AtualizarAsync(FornecedorInsumoDto dto)
        {
            var entidade = _mapper.Map<FornecedorInsumo>(dto);
            _unitOfWork.FornecedorInsumoRepository.Update(entidade);
            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var entidade = await _unitOfWork.FornecedorInsumoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.FornecedorInsumoRepository.Remove(entidade);
            await _unitOfWork.CommitAsync();

        }
    }

}
