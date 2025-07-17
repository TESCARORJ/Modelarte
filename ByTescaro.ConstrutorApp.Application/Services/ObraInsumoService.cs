using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoService : IObraInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraInsumoService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraInsumoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraInsumoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraInsumoDto>>(itens);
        }

        public async Task CriarAsync(ObraInsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<ObraInsumo>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraInsumoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraInsumoDto dto)
        {
            // Obtém o ID do usuário logado (usando await para obter Task<T>.Result de forma segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraInsumoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraInsumoAntigoParaAuditoria = await _unitOfWork.ObraInsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraInsumoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Insumo com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraInsumoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraInsumoParaAtualizar = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(dto.Id);

            if (obraInsumoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraInsumoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Insumo com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraInsumoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraInsumoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraInsumoAntigoParaAuditoria'.
            // Exemplo:
            // obraInsumoParaAtualizar.UsuarioCadastroId = obraInsumoAntigoParaAuditoria.UsuarioCadastroId;
            // obraInsumoParaAtualizar.DataHoraCadastro = obraInsumoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraInsumoRepository.Update(obraInsumoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraInsumoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraInsumoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraInsumoAntigoParaAuditoria, obraInsumoParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraInsumoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraInsumoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task<List<InsumoDto>> ObterInsumosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosDisponiveisAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

        public async Task<List<InsumoDto>> ObterInsumosPorPadraoObraAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraInsumoRepository.GetInsumosPorPadraoObraAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

    }
}
