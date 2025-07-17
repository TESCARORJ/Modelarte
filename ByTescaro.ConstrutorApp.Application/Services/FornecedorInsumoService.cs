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
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'fornecedorAntigoParaAuditoria' NÃO será modificada pelo AutoMapper.
            var fornecedorAntigoParaAuditoria = await _unitOfWork.FornecedorInsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (fornecedorAntigoParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Fornecedor de insumo com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'fornecedorParaAtualizar' é a que o EF Core está monitorando.
            var fornecedorParaAtualizar = await _unitOfWork.FornecedorInsumoRepository.GetByIdAsync(dto.Id);

            if (fornecedorParaAtualizar == null)
            {
                // Isso deve ser raro se 'fornecedorAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Fornecedor de insumo com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'fornecedorParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, fornecedorParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'fornecedorAntigoParaAuditoria':
            // fornecedorParaAtualizar.UsuarioCadastroId = fornecedorAntigoParaAuditoria.UsuarioCadastroId;
            // fornecedorParaAtualizar.DataHoraCadastro = fornecedorAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.FornecedorInsumoRepository.Update(fornecedorParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'fornecedorAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'fornecedorParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(fornecedorAntigoParaAuditoria, fornecedorParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
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
