using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces.ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraPendenciaService : IObraPendenciaService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraPendenciaService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraPendenciaDto>> ObterPorObraIdAsync(long obraId)
        {
            var list = await _unitOfWork.ObraPendenciaRepository.GetByObraIdAsync(obraId);
            return _mapper.Map<List<ObraPendenciaDto>>(list);
        }

        public async Task CriarAsync(ObraPendenciaDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraPendencia>(dto); 
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraPendenciaRepository.Add(entity);

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(ObraPendenciaDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraPendenciaAntigaParaAuditoria' NÃO será modificada e representa o estado original.
            var obraPendenciaAntigaParaAuditoria = await _unitOfWork.ObraPendenciaRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraPendenciaAntigaParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Pendência com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraPendenciaParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraPendenciaParaAtualizar = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(dto.Id);

            if (obraPendenciaParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraPendenciaAntigaParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Pendência com ID {dto.Id} não encontrada para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraPendenciaParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraPendenciaParaAtualizar);

            // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
            // Eles vêm da entidade original não modificada.
            obraPendenciaParaAtualizar.UsuarioCadastroId = obraPendenciaAntigaParaAuditoria.UsuarioCadastroId;
            obraPendenciaParaAtualizar.DataHoraCadastro = obraPendenciaAntigaParaAuditoria.DataHoraCadastro;

            // O método .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraPendenciaRepository.Update(obraPendenciaParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraPendenciaAntigaParaAuditoria' tem os dados ANTES da mudança.
            // 'obraPendenciaParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraPendenciaAntigaParaAuditoria, obraPendenciaParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraPendenciaRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraPendenciaRepository.Remove(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }
    }
}
