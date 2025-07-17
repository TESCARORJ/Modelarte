using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoInsumoService : IObraItemEtapaPadraoInsumoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraItemEtapaPadraoInsumoService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ObraItemEtapaPadraoInsumoDto>> ObterPorItemPadraoIdAsync(long itemPadraoId)
        {
            var lista = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByItemPadraoIdAsync(itemPadraoId);
            return _mapper.Map<List<ObraItemEtapaPadraoInsumoDto>>(lista);
        }

        public async Task CriarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = _mapper.Map<ObraItemEtapaPadraoInsumo>(dto);
            entidade.DataHoraCadastro = DateTime.Now;
            entidade.UsuarioCadastroId = usuarioLogadoId;
            _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Add(entidade);
            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraItemEtapaPadraoInsumoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraItemEtapaPadraoInsumoAntigoParaAuditoria = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraItemEtapaPadraoInsumoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Item Etapa Padrão Insumo com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraItemEtapaPadraoInsumoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraItemEtapaPadraoInsumoParaAtualizar = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(dto.Id);

            if (obraItemEtapaPadraoInsumoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraItemEtapaPadraoInsumoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Item Etapa Padrão Insumo com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraItemEtapaPadraoInsumoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraItemEtapaPadraoInsumoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraItemEtapaPadraoInsumoAntigoParaAuditoria'.
            // Exemplo:
            // obraItemEtapaPadraoInsumoParaAtualizar.UsuarioCadastroId = obraItemEtapaPadraoInsumoAntigoParaAuditoria.UsuarioCadastroId;
            // obraItemEtapaPadraoInsumoParaAtualizar.DataHoraCadastro = obraItemEtapaPadraoInsumoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Update(obraItemEtapaPadraoInsumoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraItemEtapaPadraoInsumoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraItemEtapaPadraoInsumoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraItemEtapaPadraoInsumoAntigoParaAuditoria, obraItemEtapaPadraoInsumoParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.ObraItemEtapaPadraoInsumoRepository.GetByIdAsync(id);
            if (entidade != null)
                _unitOfWork.ObraItemEtapaPadraoInsumoRepository.Remove(entidade);
            
            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }
    }
}
