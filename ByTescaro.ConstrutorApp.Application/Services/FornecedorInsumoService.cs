using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorInsumoService : IFornecedorInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IAuditoriaService _auditoriaService;


        public FornecedorInsumoService(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IUsuarioLogadoService usuarioLogadoService, 
            IAuditoriaService auditoriaService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
        }

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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = _mapper.Map<FornecedorInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastroId = usuarioLogado == null ? 0 : usuarioLogado.Id ;
            _unitOfWork.FornecedorInsumoRepository.Add(entidade);

            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
            return entidade.Id;
        }

        public async Task AtualizarAsync(FornecedorInsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidadeExistente = await _unitOfWork.FornecedorInsumoRepository.GetByIdAsync(dto.Id);

            var entidade = _mapper.Map<FornecedorInsumo>(dto);
            _unitOfWork.FornecedorInsumoRepository.Update(entidade);
            await _auditoriaService.RegistrarAtualizacaoAsync(entidadeExistente, entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.FornecedorInsumoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.FornecedorInsumoRepository.Remove(entidade);

            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }
    }

}
