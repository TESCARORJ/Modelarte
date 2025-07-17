using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraRetrabalhoService : IObraRetrabalhoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraRetrabalhoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraRetrabalhoDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraRetrabalhoRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraRetrabalhoDto>>(list);
        }

        public async Task CriarAsync(ObraRetrabalhoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraRetrabalho>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraRetrabalhoRepository.Add(entity);

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(ObraRetrabalhoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraRetrabalhoAntigoParaAuditoria' NÃO será modificada e representa o estado original.
            var obraRetrabalhoAntigoParaAuditoria = await _unitOfWork.ObraRetrabalhoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraRetrabalhoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Retrabalho com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraRetrabalhoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraRetrabalhoParaAtualizar = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(dto.Id);

            if (obraRetrabalhoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraRetrabalhoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Retrabalho com ID {dto.Id} não encontrada para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraRetrabalhoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraRetrabalhoParaAtualizar);

            // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
            // Eles vêm da entidade original não modificada.
            obraRetrabalhoParaAtualizar.UsuarioCadastroId = obraRetrabalhoAntigoParaAuditoria.UsuarioCadastroId;
            obraRetrabalhoParaAtualizar.DataHoraCadastro = obraRetrabalhoAntigoParaAuditoria.DataHoraCadastro;

            // O método .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraRetrabalhoRepository.Update(obraRetrabalhoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraRetrabalhoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraRetrabalhoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraRetrabalhoAntigoParaAuditoria, obraRetrabalhoParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraRetrabalhoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraRetrabalhoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }
    }
}
