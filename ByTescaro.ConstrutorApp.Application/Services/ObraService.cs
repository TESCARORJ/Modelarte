using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraService : IObraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IAuditoriaService _auditoriaService;

        public ObraService(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioLogadoService usuarioLogadoService, IAuditoriaService auditoriaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<ObraDto>> ObterPorProjetoAsync(long projetoId)
        {
            var obras = await _unitOfWork.ObraRepository.GetByProjetoIdAsync(projetoId);
            return _mapper.Map<List<ObraDto>>(obras);
        }

        public async Task<ObraDto?> ObterPorIdAsync(long id)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(id);
            return _mapper.Map<ObraDto>(obra);
        }

        public async Task<ObraDto> CriarAsync(ObraDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = _mapper.Map<Obra>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<ObraDto>(entity);


        }

        public async Task AtualizarAsync(ObraDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraAntigaParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraAntigaParaAuditoria = await _unitOfWork.ObraRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraAntigaParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraParaAtualizar = await _unitOfWork.ObraRepository.GetByIdAsync(dto.Id);

            if (obraParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraAntigaParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra com ID {dto.Id} não encontrada para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraAntigaParaAuditoria'.
            // Exemplo:
            // obraParaAtualizar.UsuarioCadastroId = obraAntigaParaAuditoria.UsuarioCadastroId;
            // obraParaAtualizar.DataHoraCadastro = obraAntigaParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraRepository.Update(obraParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraAntigaParaAuditoria' tem os dados ANTES da mudança.
            // 'obraParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraAntigaParaAuditoria, obraParaAtualizar, usuarioLogadoId);

            // 5. Salva TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }

        public async Task<int> CalcularProgressoAsync(long obraId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(obraId);
            if (obra == null || obra.Etapas.Count == 0)
                return 0;

            int progressoTotal = obra.Etapas.Sum(e => e.Itens.Count == 0 ? 0 :
                (int)Math.Round((double)e.Itens.Count(i => i.Concluido) / e.Itens.Count * 100));

            return (int)Math.Round((double)progressoTotal / obra.Etapas.Count);
        }

        public async Task<List<ObraDto>> ObterTodosAsync()
        {
            var obras = await _unitOfWork.ObraRepository.GetAllAsync();
            return _mapper.Map<List<ObraDto>>(obras);
        }

        public async Task<List<ObraEtapaDto>> ObterEtapasDaObraAsync(long obraId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(obraId);
            return _mapper.Map<List<ObraEtapaDto>>(obra?.Etapas?.ToList() ?? new List<ObraEtapa>());
        }

        public async Task<List<ObraItemEtapaDto>> ObterItensDaEtapaAsync(long etapaId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(etapaId);
            var etapa = obra?.Etapas.FirstOrDefault(e => e.Id == etapaId);
            var itens = etapa?.Itens?.ToList() ?? new List<ObraItemEtapa>();
            return _mapper.Map<List<ObraItemEtapaDto>>(itens);
        }


        public async Task AtualizarConclusaoItemAsync(long itemId, bool concluido)
        {
            var obra = await _unitOfWork.ObraRepository.GetByItemEtapaIdAsync(itemId);
            var item = obra?.Etapas.SelectMany(e => e.Itens).FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Concluido = concluido;
                await _unitOfWork.ObraRepository.UpdateAsync(obra!);
            }
        }

    }

}
