using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class ServicoService : IServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;


        public ServicoService(
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



        public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.ServicoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServicoDto>>(insumos);
        }

        public async Task<ServicoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<ServicoDto>(insumo);
        }

        public async Task CriarAsync(ServicoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Servico>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ServicoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ServicoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'servicoAntigoParaAuditoria' NÃO será modificada e representa o estado original.
            var servicoAntigoParaAuditoria = await _unitOfWork.ServicoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (servicoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Serviço com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'servicoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var servicoParaAtualizar = await _unitOfWork.ServicoRepository.GetByIdAsync(dto.Id);

            if (servicoParaAtualizar == null)
            {
                // Isso deve ser raro se 'servicoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Serviço com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'servicoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, servicoParaAtualizar);

            // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
            // Eles vêm da entidade original não modificada.
            // CUIDADO: Você tinha entityNovo.UsuarioCadastroId = entityAntigo.Id; na sua versão original.
            // O correto aqui é servicoAntigoParaAuditoria.UsuarioCadastroId (o ID do usuário que CADASTRou).
            servicoParaAtualizar.UsuarioCadastroId = servicoAntigoParaAuditoria.UsuarioCadastroId;
            servicoParaAtualizar.DataHoraCadastro = servicoAntigoParaAuditoria.DataHoraCadastro;
            // Adicione campos de auditoria de atualização se existirem na entidade Servico



            // O método .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ServicoRepository.Update(servicoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'servicoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'servicoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(servicoAntigoParaAuditoria, servicoParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.ServicoRepository.Remove(entity);
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.ServicoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}