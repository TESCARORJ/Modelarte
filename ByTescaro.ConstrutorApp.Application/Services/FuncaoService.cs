using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FuncaoService : IFuncaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        public FuncaoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<IEnumerable<FuncaoDto>> ObterTodasAsync()
        {
            var funcoes = await _unitOfWork.FuncaoRepository.ObterTodasAsync();
            return _mapper.Map<IEnumerable<FuncaoDto>>(funcoes);
        }

        public async Task<FuncaoDto?> ObterPorIdAsync(long id)
        {
            var funcao = await _unitOfWork.FuncaoRepository.GetByIdAsync(id);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task<FuncaoDto?> ObterPorNomeAsync(string nome)
        {
            var funcao = await _unitOfWork.FuncaoRepository.ObterPorNomeAsync(nome);
            return funcao == null ? null : _mapper.Map<FuncaoDto>(funcao);
        }

        public async Task CriarAsync(FuncaoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Funcao>(dto);
            _unitOfWork.FuncaoRepository.Add(entity);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(FuncaoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            
            var antigaParaAuditoria = await _unitOfWork.FuncaoRepository.GetByIdNoTrackingAsync(dto.Id); //

            if (antigaParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Função com ID {dto.Id} não encontrada para auditoria.");
            }

            var dadosAnteriores = JsonSerializer.Serialize(_mapper.Map<FuncaoDto>(antigaParaAuditoria)); 

            
            var funcaoParaAtualizar = await _unitOfWork.FuncaoRepository.GetByIdTrackingAsync(dto.Id); 

            if (funcaoParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Função com ID {dto.Id} não encontrada para atualização.");
            }

           
            _mapper.Map(dto, funcaoParaAtualizar);

           
            funcaoParaAtualizar.UsuarioCadastroId = antigaParaAuditoria.UsuarioCadastroId;
            funcaoParaAtualizar.DataHoraCadastro = antigaParaAuditoria.DataHoraCadastro;

            _unitOfWork.FuncaoRepository.Update(funcaoParaAtualizar);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(antigaParaAuditoria, funcaoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.FuncaoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.FuncaoRepository.Remove(entity);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
        }

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return false; // Nome vazio não é considerado duplicado
            }
            return await _unitOfWork.FuncaoRepository.ExistsAsync(f =>
                f.Nome == nome && (ignoreId == null || f.Id != ignoreId.Value));
        }
    }

}