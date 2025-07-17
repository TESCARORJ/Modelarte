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
        public FuncaoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(FuncaoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'antigaParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log.
            var antigaParaAuditoria = await _unitOfWork.FuncaoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (antigaParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Função com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'funcaoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas.
            var funcaoParaAtualizar = await _unitOfWork.FuncaoRepository.GetByIdAsync(dto.Id);

            if (funcaoParaAtualizar == null)
            {
                // Isso deve ser raro se 'antigaParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Função com ID {dto.Id} não encontrada para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'funcaoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, funcaoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'antigaParaAuditoria':
            // funcaoParaAtualizar.UsuarioCadastroId = antigaParaAuditoria.UsuarioCadastroId;
            // funcaoParaAtualizar.DataHoraCadastro = antigaParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.FuncaoRepository.Update(funcaoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'antigaParaAuditoria' tem os dados ANTES da mudança.
            // 'funcaoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(antigaParaAuditoria, funcaoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.FuncaoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.FuncaoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
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