using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoService : IOrcamentoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public OrcamentoService(IMapper mapper, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<OrcamentoDto> CriarAsync(OrcamentoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            // Calcular total com base nos itens
            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            var entidade = _mapper.Map<Orcamento>(dto);
            entidade.UsuarioCadastroId = usuarioLogadoId;
            entidade.DataHoraCadastro = DateTime.Now;

            // Preencher metadados dos itens
            foreach (var item in entidade.Itens)
            {
                item.UsuarioCadastroId = usuarioLogadoId;
                item.DataHoraCadastro = DateTime.Now;
            }

            _unitOfWork.OrcamentoRepository.Add(entidade);

            await _auditoriaService.RegistrarCriacaoAsync(entidade, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

            return _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<List<OrcamentoDto>> ObterTodosAsync()
        {
            var lista = await _unitOfWork.OrcamentoRepository.GetAllAsync();
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<List<OrcamentoDto>> ObterPorObraAsync(long obraId)
        {
            var lista = await _unitOfWork.OrcamentoRepository.GetByObraAsync(obraId);
            return _mapper.Map<List<OrcamentoDto>>(lista);
        }

        public async Task<OrcamentoDto?> ObterPorIdAsync(long id)
        {
            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task<OrcamentoDto?> ObterPorIdComItensAsync(long id)
        {
            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdComItensAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoDto>(entidade);
        }

        public async Task AtualizarAsync(OrcamentoDto dto)
        {
            // Obtém o ID do usuário logado (usando 'await' para obter o resultado da Task de forma assíncrona e segura)
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga com os itens, SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'orcamentoAntigoParaAuditoria' NÃO será modificada e representa o estado original.
            var orcamentoAntigoParaAuditoria = await _unitOfWork.OrcamentoRepository
                .GetByIdComItensNoTrackingAsync(dto.Id); // NOVO MÉTODO NECESSÁRIO

            if (orcamentoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Orçamento com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO e com os itens.
            // Essa instância 'orcamentoParaAtualizar' é a que o EF Core está monitorando e será modificada.
            var orcamentoParaAtualizar = await _unitOfWork.OrcamentoRepository
                .GetByIdComItensAsync(dto.Id);

            if (orcamentoParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Orçamento com ID {dto.Id} não encontrado para atualização.");
            }

            // Validação de itens
            if (dto.Itens == null || !dto.Itens.Any())
                throw new ArgumentException("O orçamento deve conter pelo menos um item.");

            // Atualiza o total com base nos itens do DTO
            dto.Total = dto.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            // 3. Mapeie as propriedades do DTO para a entidade 'orcamentoParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, orcamentoParaAtualizar);

            // Reatribua os campos de auditoria de criação que não devem ser alterados pelo DTO.
            // Eles vêm da entidade original não modificada.
            orcamentoParaAtualizar.UsuarioCadastroId = orcamentoAntigoParaAuditoria.UsuarioCadastroId;
            orcamentoParaAtualizar.DataHoraCadastro = orcamentoAntigoParaAuditoria.DataHoraCadastro;

            // 4. Lógica para ATUALIZAR/ADICIONAR/REMOVER itens da coleção 'Itens'.
            // Em vez de criar uma nova coleção, vamos comparar e modificar apenas o necessário.

            // Itens a serem removidos (existem na lista antiga, mas não no DTO)
            var itensParaRemover = orcamentoParaAtualizar.Itens
                .Where(existingItem => !dto.Itens.Any(dtoItem => dtoItem.Id == existingItem.Id && dtoItem.Id != 0))
                .ToList();

            foreach (var item in itensParaRemover)
            {
                orcamentoParaAtualizar.Itens.Remove(item);
                // Se você tiver um repositório específico para OrcamentoItem, pode ser bom marcar para remoção explícita:
                // _unitOfWork.OrcamentoItemRepository.Remove(item);
            }

            // Itens a serem adicionados ou atualizados
            foreach (var itemDto in dto.Itens)
            {
                var existingItem = orcamentoParaAtualizar.Itens.FirstOrDefault(i => i.Id == itemDto.Id && i.Id != 0);

                if (existingItem == null) // Item novo (não tem ID ou não foi encontrado na lista existente)
                {
                    orcamentoParaAtualizar.Itens.Add(new OrcamentoItem
                    {
                        ServicoId = itemDto.ServicoId, // Ou InsumoId, dependendo do seu DTO
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitario = itemDto.PrecoUnitario,
                        DataHoraCadastro = DateTime.Now,
                        UsuarioCadastroId = usuarioLogadoId,
                        // Certifique-se de que a FK para Orcamento seja preenchida se não for feita por convenção do EF
                        OrcamentoObraId = orcamentoParaAtualizar.Id
                    });
                }
                else // Item existente, atualizar
                {
                    // Atualize as propriedades que podem mudar
                    existingItem.ServicoId = itemDto.ServicoId; // Ou InsumoId
                    existingItem.Quantidade = itemDto.Quantidade;
                    existingItem.PrecoUnitario = itemDto.PrecoUnitario;
    

                    // Não é necessário chamar _unitOfWork.OrcamentoItemRepository.Update(existingItem);
                    // O EF Core já está rastreando existingItem e detectará as mudanças.
                }
            }

            // O _unitOfWork.OrcamentoRepository.Update(entityNovo); é geralmente redundante aqui,
            // pois a entidade principal já está rastreada e suas propriedades e a coleção Itens foram modificadas.
            // O EF Core detectará as mudanças automaticamente.
            // _unitOfWork.OrcamentoRepository.Update(orcamentoParaAtualizar);

            // 5. Registre a auditoria. Use RegistrarAtualizacaoAsync.
            // Certifique-se de que o seu AuditoriaService saiba como lidar com coleções aninhadas
            // ao comparar 'antigo' e 'novo'.
            await _auditoriaService.RegistrarAtualizacaoAsync(orcamentoAntigoParaAuditoria, orcamentoParaAtualizar, usuarioLogadoId);

            // 6. Salva TODAS as alterações no banco de dados em uma única transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entidade = await _unitOfWork.OrcamentoRepository.GetByIdComItensAsync(id);
            if (entidade == null) return;

            _unitOfWork.OrcamentoRepository.Remove(entidade);

            await _auditoriaService.RegistrarExclusaoAsync(entidade, usuarioLogadoId);
            await _unitOfWork.CommitAsync();
        }
    }
}
