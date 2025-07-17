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

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'fornecedorServicoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log.
            var fornecedorServicoAntigoParaAuditoria = await _unitOfWork.FornecedorServicoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (fornecedorServicoAntigoParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Fornecedor de serviço com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'fornecedorServicoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas.
            var fornecedorServicoParaAtualizar = await _unitOfWork.FornecedorServicoRepository.GetByIdAsync(dto.Id);

            if (fornecedorServicoParaAtualizar == null)
            {
                // Isso deve ser raro se 'fornecedorServicoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Fornecedor de serviço com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'fornecedorServicoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, fornecedorServicoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'fornecedorServicoAntigoParaAuditoria':
            // fornecedorServicoParaAtualizar.UsuarioCadastroId = fornecedorServicoAntigoParaAuditoria.UsuarioCadastroId;
            // fornecedorServicoParaAtualizar.DataHoraCadastro = fornecedorServicoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.FornecedorServicoRepository.Update(fornecedorServicoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'fornecedorServicoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'fornecedorServicoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(fornecedorServicoAntigoParaAuditoria, fornecedorServicoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
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
