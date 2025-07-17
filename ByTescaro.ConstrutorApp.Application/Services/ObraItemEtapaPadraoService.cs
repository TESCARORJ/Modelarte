using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaPadraoService : IObraItemEtapaPadraoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraItemEtapaPadraoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<ObraItemEtapaPadraoDto?> ObterPorIdAsync(long id)
        {
            var entity = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(id);
            return _mapper.Map<ObraItemEtapaPadraoDto>(entity);
        }
        public async Task<List<ObraItemEtapaPadraoDto>> ObterTodasAsync()
        {
            var list = await _unitOfWork.ObraItemEtapaPadraoRepository.FindAllWithIncludesAsync(x => x.Id > 0, x => x.ObraEtapaPadrao);
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(list);
        }
        public async Task<List<ObraItemEtapaPadraoDto>> ObterPorEtapaIdAsync(long etapaId)
        {
            var itens = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByEtapaPadraoIdAsync(etapaId);
            return _mapper.Map<List<ObraItemEtapaPadraoDto>>(itens);
        }
        public async Task AtualizarConclusaoAsync(long itemId, bool concluido)
        {
            var item = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(itemId);
            if (item == null) return;

            // Como este é um item padrão, normalmente ele não teria um campo "Concluido"
            // Este método deve ser ignorado ou removido da interface
            // Caso queira manter, você deve adicionar esse campo na entidade
        }
        public async Task<ObraItemEtapaPadraoDto> CriarAsync(ObraItemEtapaPadraoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
           
            dto.Nome = dto.Nome.Trim();


            // VERIFICAÇÃO DE DUPLICIDADE
            if (await _unitOfWork.ObraItemEtapaPadraoRepository.JaExisteAsync(dto.Nome, dto.ObraEtapaPadraoId))
            {
                throw new DuplicateRecordException($"O item '{dto.Nome}' já existe para esta etapa padrão.");

            }

            var entity = _mapper.Map<ObraItemEtapaPadrao>(dto);


            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraItemEtapaPadraoRepository.Add(entity); // O 'entity' agora terá o ID gerado após o SaveChanges interno do AddAsync
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

            // Mapeia a entidade atualizada de volta para um DTO e o retorna
            var createdDto = _mapper.Map<ObraItemEtapaPadraoDto>(entity);
            return createdDto;
        }

        public async Task AtualizarAsync(ObraItemEtapaPadraoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // Limpa espaços em branco do nome
            dto.Nome = dto.Nome.Trim();

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraItemEtapaPadraoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraItemEtapaPadraoAntigoParaAuditoria = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraItemEtapaPadraoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Item Etapa Padrão com ID {dto.Id} não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraItemEtapaPadraoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraItemEtapaPadraoParaAtualizar = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(dto.Id);

            if (obraItemEtapaPadraoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraItemEtapaPadraoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Item Etapa Padrão com ID {dto.Id} não encontrada para atualização.");
            }

            // VERIFICAÇÃO DE DUPLICIDADE (ignorando o próprio ID)
            if (await _unitOfWork.ObraItemEtapaPadraoRepository.JaExisteAsync(dto.Nome, dto.ObraEtapaPadraoId, dto.Id))
            {
                throw new DuplicateRecordException($"O item '{dto.Nome}' já existe para esta etapa padrão.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraItemEtapaPadraoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraItemEtapaPadraoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraItemEtapaPadraoAntigoParaAuditoria'.
            // Exemplo:
            // obraItemEtapaPadraoParaAtualizar.UsuarioCadastroId = obraItemEtapaPadraoAntigoParaAuditoria.UsuarioCadastroId;
            // obraItemEtapaPadraoParaAtualizar.DataHoraCadastro = obraItemEtapaPadraoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraItemEtapaPadraoRepository.Update(obraItemEtapaPadraoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraItemEtapaPadraoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraItemEtapaPadraoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraItemEtapaPadraoAntigoParaAuditoria, obraItemEtapaPadraoParaAtualizar, usuarioLogadoId);

            // 5. Salva TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraItemEtapaPadraoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraItemEtapaPadraoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

    }
}
