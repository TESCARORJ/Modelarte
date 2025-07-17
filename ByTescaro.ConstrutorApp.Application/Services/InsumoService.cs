using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class InsumoService : IInsumoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public InsumoService(
            IAuditoriaService auditoriaService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUsuarioLogadoService usuarioLogadoService)
        {
            _auditoriaService = auditoriaService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<IEnumerable<InsumoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.InsumoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InsumoDto>>(insumos);
        }

        public async Task<InsumoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<InsumoDto>(insumo);
        }

        public async Task CriarAsync(InsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Insumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.InsumoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(InsumoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'insumoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log.
            var insumoAntigoParaAuditoria = await _unitOfWork.InsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (insumoAntigoParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Insumo com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'insumoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas.
            var insumoParaAtualizar = await _unitOfWork.InsumoRepository.GetByIdAsync(dto.Id);

            if (insumoParaAtualizar == null)
            {
                // Isso deve ser raro se 'insumoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Insumo com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'insumoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, insumoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'insumoAntigoParaAuditoria':
            insumoParaAtualizar.UsuarioCadastroId = insumoAntigoParaAuditoria.UsuarioCadastroId;
            insumoParaAtualizar.DataHoraCadastro = insumoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.InsumoRepository.Update(insumoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'insumoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'insumoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(insumoAntigoParaAuditoria, insumoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
            await _unitOfWork.CommitAsync();
        }
        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.InsumoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.InsumoRepository.Remove(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }


        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.InsumoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}