using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraChecklistService : IObraChecklistService
    {
        private readonly IObraEtapaRepository _etapaRepo;
        private readonly IObraItemEtapaRepository _itemRepo;
        private readonly IObraRepository _obraRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ObraChecklistService> _logger;
        private readonly IFuncionarioRepository funcionarioRepository;


        public ObraChecklistService(IObraEtapaRepository etapaRepo, IObraItemEtapaRepository itemRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor, INotificationService notificationService, IObraRepository obraRepository, ILogger<ObraChecklistService> logger, IFuncionarioRepository funcionarioRepository)
        {
            _etapaRepo = etapaRepo;
            _itemRepo = itemRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _obraRepository = obraRepository;
            _logger = logger;
            this.funcionarioRepository = funcionarioRepository;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<List<ObraEtapaDto>> ObterChecklistAsync(long obraId)
        {
            var etapas = await _etapaRepo.GetByObraIdAsync(obraId);
            foreach (var etapa in etapas)
                etapa.Itens = await _itemRepo.GetByEtapaIdAsync(etapa.Id);

            return _mapper.Map<List<ObraEtapaDto>>(etapas);
        }

        //public async Task SalvarChecklistAsync(long obraId, List<ObraEtapaDto> etapasDto)
        //{
        //    var etapasAtuais = await _etapaRepo.GetByObraIdAsync(obraId);

        //    // Remoção
        //    var removidas = etapasAtuais.Where(e => !etapasDto.Any(dto => dto.Id == e.Id)).ToList();
        //    foreach (var etapa in removidas)
        //        await _etapaRepo.RemoveAsync(etapa);

        //    // Adição/Atualização
        //    foreach (var etapaDto in etapasDto)
        //    {
        //        if (etapaDto.Id == 0)
        //        {
        //            var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
        //            novaEtapa.ObraId = obraId;
        //            novaEtapa.Itens = new List<ObraItemEtapa>(); // evita duplicação automática

        //            await _etapaRepo.AddAsync(novaEtapa);

        //            foreach (var itemDto in etapaDto.Itens)
        //            {
        //                var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
        //                novoItem.ObraEtapaId = novaEtapa.Id;
        //                await _itemRepo.AddAsync(novoItem);
        //            }
        //        }

        //        else
        //        {
        //            var etapaExistente = etapasAtuais.FirstOrDefault(e => e.Id == etapaDto.Id);
        //            if (etapaExistente is null) continue;

        //            etapaExistente.Nome = etapaDto.Nome;
        //            etapaExistente.Ordem = etapaDto.Ordem;
        //            etapaExistente.Status = etapaDto.Status;
        //            etapaExistente.DataInicio = etapaDto.DataInicio;
        //            etapaExistente.DataConclusao = etapaDto.DataConclusao;

        //            await _etapaRepo.UpdateAsync(etapaExistente);

        //            var itensAtuais = await _itemRepo.GetByEtapaIdAsync(etapaExistente.Id);

        //            var itensRemovidos = itensAtuais
        //                .Where(i => !etapaDto.Itens.Any(dto => dto.Id == i.Id)).ToList();

        //            foreach (var item in itensRemovidos)
        //                await _itemRepo.RemoveAsync(item);

        //            foreach (var itemDto in etapaDto.Itens)
        //            {
        //                if (itemDto.Id == 0)
        //                {
        //                    var novo = _mapper.Map<ObraItemEtapa>(itemDto);
        //                    novo.ObraEtapaId = etapaExistente.Id;
        //                    await _itemRepo.AddAsync(novo);
        //                }
        //                else
        //                {
        //                    var existente = itensAtuais.FirstOrDefault(i => i.Id == itemDto.Id);
        //                    if (existente != null)
        //                    {
        //                        existente.Nome = itemDto.Nome;
        //                        existente.Ordem = itemDto.Ordem;
        //                        existente.Concluido = itemDto.Concluido;
        //                        existente.IsDataPrazo = itemDto.IsDataPrazo;
        //                        existente.DiasPrazo = itemDto.DiasPrazo;
        //                        existente.PrazoAtivo = itemDto.PrazoAtivo;
        //                        existente.DataConclusao = itemDto.DataConclusao;
        //                        await _itemRepo.UpdateAsync(existente);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Sincroniza o checklist de uma obra, atualizando, adicionando ou removendo etapas e itens
        /// conforme o estado recebido. Envia notificações para o cliente quando um item é marcado como concluído.
        /// </summary>
        /// <param name="obraId">O ID da obra que está sendo atualizada.</param>
        /// <param name="etapasDto">A lista de etapas e itens que representa o estado final desejado para o checklist.</param>
        public async Task SalvarChecklistAsync(long obraId, List<ObraEtapaDto> etapasDto)
        {
            // 1. OTIMIZAÇÃO: Carregar dados para notificação uma única vez
            // Buscamos a obra com todos os relacionamentos necessários (Projeto e Cliente)
            // antes de iniciar os loops para evitar consultas repetitivas ao banco.
            var obraParaNotificacao = await _obraRepository.GetByIdAsync(obraId);
           

            // Se não for possível carregar a obra ou cliente, apenas logamos um aviso.
            // O processo de salvar o checklist continua, mas sem enviar notificações.
            if (obraParaNotificacao?.Projeto?.Cliente == null)
            {
                // Exemplo de log (requer injeção de ILogger)
                // _logger.LogWarning("Dados da obra {ObraId} ou cliente associado não encontrados. Nenhuma notificação será enviada.", obraId);
            }

            // 2. SINCRONIZAÇÃO: Buscar o estado atual do checklist no banco de dados
            var etapasAtuais = await _etapaRepo.GetByObraIdAsync(obraId);

            // 3. SINCRONIZAÇÃO (REMOÇÃO): Remover etapas que não vieram no DTO
            var etapasRemovidas = etapasAtuais.Where(e => !etapasDto.Any(dto => dto.Id == e.Id)).ToList();
            foreach (var etapa in etapasRemovidas)
            {
                // O repositório deve cuidar da remoção em cascata dos itens ou isso deve ser feito manualmente.
                // Assumindo que o repositório ou o banco de dados lida com isso.
                await _etapaRepo.RemoveAsync(etapa);
            }

            // 4. SINCRONIZAÇÃO (ADIÇÃO/ATUALIZAÇÃO): Processar cada etapa do DTO
            foreach (var etapaDto in etapasDto)
            {
                if (etapaDto.Id == 0) // É uma nova etapa
                {
                    var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
                    novaEtapa.ObraId = obraId;
                    novaEtapa.Itens = new List<ObraItemEtapa>(); // Prevenir problemas com AutoMapper

                    await _etapaRepo.AddAsync(novaEtapa);

                    // Adicionar os itens desta nova etapa
                    foreach (var itemDto in etapaDto.Itens)
                    {
                        var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                        novoItem.ObraEtapaId = novaEtapa.Id; // Lincar com o ID da etapa recém-criada
                        await _itemRepo.AddAsync(novoItem);
                    }
                }
                else // É uma etapa existente
                {
                    var etapaExistente = etapasAtuais.FirstOrDefault(e => e.Id == etapaDto.Id);
                    if (etapaExistente == null) continue; // Etapa não encontrada, pular

                    // Atualizar os dados da etapa
                    _mapper.Map(etapaDto, etapaExistente);
                    await _etapaRepo.UpdateAsync(etapaExistente);

                    // Sincronizar os itens desta etapa
                    var itensAtuaisDaEtapa = await _itemRepo.GetByEtapaIdAsync(etapaExistente.Id);

                    // Remover itens que não vieram no DTO
                    var itensRemovidos = itensAtuaisDaEtapa.Where(i => !etapaDto.Itens.Any(dto => dto.Id == i.Id)).ToList();
                    foreach (var item in itensRemovidos)
                    {
                        await _itemRepo.RemoveAsync(item);
                    }

                    // Adicionar ou atualizar itens
                    foreach (var itemDto in etapaDto.Itens)
                    {
                        if (itemDto.Id == 0) // Novo item
                        {
                            var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                            novoItem.ObraEtapaId = etapaExistente.Id;
                            await _itemRepo.AddAsync(novoItem);
                        }
                        else // Item existente
                        {
                            var itemExistente = itensAtuaisDaEtapa.FirstOrDefault(i => i.Id == itemDto.Id);
                            if (itemExistente != null)
                            {
                                // LÓGICA DE NOTIFICAÇÃO
                                bool eraConcluido = itemExistente.Concluido;

                                _mapper.Map(itemDto, itemExistente);
                                await _itemRepo.UpdateAsync(itemExistente);

                                bool foiConcluidoAgora = itemExistente.Concluido && !eraConcluido;

                                // Se o item foi concluído nesta transação e temos os dados da obra, notificar.
                                if (foiConcluidoAgora && obraParaNotificacao?.Projeto?.Cliente != null)
                                {
                                    // Dispara a notificação em "fire-and-forget" para não bloquear a resposta da API
                                    _ = EnviarNotificacaoDeConclusaoAsync(itemExistente, etapaExistente.Nome, obraParaNotificacao);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Método auxiliar otimizado que monta e envia a notificação via WhatsApp.
        /// Ele recebe os objetos já carregados para evitar novas consultas ao banco.
        /// </summary>
        private async Task EnviarNotificacaoDeConclusaoAsync(ObraItemEtapa itemConcluido, string nomeEtapa, Obra obra)
        {
            var cliente = obra.Projeto.Cliente;

            if (cliente == null)
            {
                _logger.LogWarning("Não foi possível encontrar o cliente para a obra {ObraId} para enviar a notificação.", obra.Id);
                return;
            }
            var responsavelObra = funcionarioRepository.GetByIdAsync(obra.ResponsavelObraId).Result;
            var telefone = responsavelObra.TelefoneWhatsApp;

            if (string.IsNullOrWhiteSpace(telefone)) return;

            var mensagem = new StringBuilder();
            mensagem.AppendLine("✨ *Notificação de Andamento da Obra* ✨");
            mensagem.AppendLine();
            mensagem.AppendLine($"*Cliente:* {cliente.Nome}");
            mensagem.AppendLine($"*Projeto:* {obra.Projeto.Nome}");
            mensagem.AppendLine($"*Obra:* {obra.Nome}");
            mensagem.AppendLine($"*Etapa:* {nomeEtapa}");
            mensagem.AppendLine($"*Item Concluído:* ✅ {itemConcluido.Nome}");
            mensagem.AppendLine();
            mensagem.AppendLine("_Esta é uma mensagem automática. Por favor, não responda._");

            await _notificationService.SendWhatsAppMessageAsync(telefone, mensagem.ToString());
        }
    }


}
