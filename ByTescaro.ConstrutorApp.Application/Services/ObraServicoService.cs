using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoService : IObraServicoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraServicoService(
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

        public async Task<List<ObraServicoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraServicoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraServicoDto>>(itens);
        }

        public async Task CriarAsync(ObraServicoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var usuarioLogadoNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome;

            dto.DataHoraCadastro = DateTime.Now;
            dto.UsuarioCadastroId = usuarioLogadoId;
            dto.UsuarioCadastroNome = usuarioLogadoNome;
            var entity = _mapper.Map<ObraServico>(dto);
            _unitOfWork.ObraServicoRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraServicoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'obraServicoAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log de auditoria.
            var obraServicoAntigoParaAuditoria = await _unitOfWork.ObraServicoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraServicoAntigoParaAuditoria == null)
            {
                // Se a entidade não foi encontrada, não há o que atualizar ou auditar.
                throw new KeyNotFoundException($"Obra Serviço com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'obraServicoParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas e salvas no banco de dados.
            var obraServicoParaAtualizar = await _unitOfWork.ObraServicoRepository.GetByIdAsync(dto.Id);

            if (obraServicoParaAtualizar == null)
            {
                // Isso deve ser raro se 'obraServicoAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança para o fluxo de atualização.
                throw new KeyNotFoundException($"Obra Serviço com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'obraServicoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, obraServicoParaAtualizar);

            // Se houver campos de auditoria de criação (UsuarioCadastroId, DataHoraCadastro)
            // que não devem ser alterados pelo DTO, você pode reatribuí-los aqui,
            // usando os valores de 'obraServicoAntigoParaAuditoria'.
            // Exemplo:
            // obraServicoParaAtualizar.UsuarioCadastroId = obraServicoAntigoParaAuditoria.UsuarioCadastroId;
            // obraServicoParaAtualizar.DataHoraCadastro = obraServicoAntigoParaAuditoria.DataHoraCadastro;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta essas mudanças automaticamente.
            // _unitOfWork.ObraServicoRepository.Update(obraServicoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'obraServicoAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'obraServicoParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(obraServicoAntigoParaAuditoria, obraServicoParaAtualizar, usuarioLogadoId);

            // 5. Salve TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = await _unitOfWork.ObraServicoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraServicoRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task<List<ServicoDto>> ObterServicosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraServicoRepository.GetServicosDisponiveisAsync(obraId);
            return _mapper.Map<List<ServicoDto>>(entidades);
        }

    }
}
