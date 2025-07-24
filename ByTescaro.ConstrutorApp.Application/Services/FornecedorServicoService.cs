using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorServicoService : IFornecedorServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public FornecedorServicoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<List<FornecedorServicoDto>> ObterTodosAsync()
        {
            var lista = await _unitOfWork.FornecedorServicoRepository.GetAllAsync();
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<FornecedorServicoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _unitOfWork.FornecedorServicoRepository.GetByIdAsync(id);
            return entidade == null ? null : _mapper.Map<FornecedorServicoDto>(entidade);
        }

        public async Task<List<FornecedorServicoDto>> ObterPorFornecedorAsync(long fornecedorId)
        {
            var lista = await _unitOfWork.FornecedorServicoRepository.ObterPorFornecedorIdAsync(fornecedorId);
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<List<FornecedorServicoDto>> ObterPorServicoAsync(long servicoId)
        {
            var lista = await _unitOfWork.FornecedorServicoRepository.ObterPorServicoIdAsync(servicoId);
            return _mapper.Map<List<FornecedorServicoDto>>(lista);
        }

        public async Task<long> CriarAsync(FornecedorServicoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entidade = _mapper.Map<FornecedorServico>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.FornecedorServicoRepository.Add(entidade);
            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
            return entidade.Id;
        }

        public async Task AtualizarAsync(FornecedorServicoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

        
            var fornecedorServicoAntigoParaAuditoria = await _unitOfWork.FornecedorServicoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (fornecedorServicoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Fornecedor de serviço com ID {dto.Id} não encontrado para auditoria.");
            }

            
            var fornecedorServicoParaAtualizar = await _unitOfWork.FornecedorServicoRepository.GetByIdAsync(dto.Id);

            if (fornecedorServicoParaAtualizar == null)
            {
         
                throw new KeyNotFoundException($"Fornecedor de serviço com ID {dto.Id} não encontrado para atualização.");
            }

            _mapper.Map(dto, fornecedorServicoParaAtualizar);

          
            _unitOfWork.FornecedorServicoRepository.Update(fornecedorServicoParaAtualizar);

     
            await _auditoriaService.RegistrarAtualizacaoAsync(fornecedorServicoAntigoParaAuditoria, fornecedorServicoParaAtualizar, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.FornecedorServicoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.FornecedorServicoRepository.Add(entidade);

            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }
    }

}
