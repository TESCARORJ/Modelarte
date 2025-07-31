using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraChecklistService : IObraChecklistService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ObraChecklistService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;


        public ObraChecklistService(IMapper mapper, IHttpContextAccessor httpContextAccessor, INotificationService notificationService, ILogger<ObraChecklistService> logger, IUnitOfWork unitOfWork, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<List<ObraEtapaDto>> ObterChecklistAsync(long obraId)
        {
            var etapasComItens = await _unitOfWork.ObraEtapaRepository.GetByObraIdAsync(obraId);

            return _mapper.Map<List<ObraEtapaDto>>(etapasComItens);
        }


        /// <summary>
        /// Sincroniza o checklist de uma obra, atualizando, adicionando ou removendo etapas e itens
        /// conforme o estado recebido. Envia notificações para o cliente quando um item é marcado como concluído.
        /// </summary>
        public async Task SalvarChecklistAsync(long obraId, List<ObraEtapaDto> etapasDto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var obraParaNotificacao = await _unitOfWork.ObraRepository.FindOneWithIncludesAsync(x => x.Id == obraId, x => x.Projeto);
            var projeto = _unitOfWork.ProjetoRepository.FindOneWithIncludesAsync(x => x.Id == obraParaNotificacao.ProjetoId, x => x.Cliente).Result;
            obraParaNotificacao.Projeto.Cliente = projeto.Cliente;

            if (obraParaNotificacao?.Projeto?.Cliente == null)
            {
                _logger.LogWarning("Dados da obra {ObraId} ou cliente associado não encontrados. Nenhuma notificação será enviada.", obraId);
            }

            // A consulta GetByObraIdAsync já deve trazer os Itens via .Include()
            var etapasAtuais = await _unitOfWork.ObraEtapaRepository.GetByObraIdAsync(obraId);

            // REMOÇÃO de etapas
            var idsEtapasDto = etapasDto.Select(dto => dto.Id).ToHashSet();
            var etapasParaRemover = etapasAtuais.Where(e => !idsEtapasDto.Contains(e.Id)).ToList();
            foreach (var etapa in etapasParaRemover)
            {
                _unitOfWork.ObraEtapaRepository.Remove(etapa);
                await _auditoriaService.RegistrarExclusaoAsync(etapa, usuarioLogadoId);

            }

            // ADIÇÃO/ATUALIZAÇÃO de etapas
            foreach (var etapaDto in etapasDto)
            {
                if (etapaDto.Id == 0) // Nova etapa
                {
                    var novaEtapa = _mapper.Map<ObraEtapa>(etapaDto);
                    novaEtapa.ObraId = obraId;
                    novaEtapa.Itens = new List<ObraItemEtapa>();
                    _unitOfWork.ObraEtapaRepository.Add(novaEtapa);

                    foreach (var itemDto in etapaDto.Itens)
                    {
                        var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                        novoItem.ObraEtapa = novaEtapa;
                        _unitOfWork.ObraItemEtapaRepository.Add(novoItem);
                        await _auditoriaService.RegistrarCriacaoAsync(novoItem, usuarioLogadoId);

                    }
                }
                else // Etapa existente
                {
                    var etapaExistente = etapasAtuais.FirstOrDefault(e => e.Id == etapaDto.Id);
                    if (etapaExistente == null) continue;

                    // Atualize apenas as propriedades da Etapa, não a coleção de Itens.
                    // Isso evita que o AutoMapper substitua a coleção rastreada.
                    etapaExistente.Nome = etapaDto.Nome;
                    etapaExistente.Ordem = etapaDto.Ordem;
                    etapaExistente.Status = etapaDto.Status;
                    etapaExistente.DataInicio = etapaDto.DataInicio;
                    etapaExistente.DataConclusao = etapaDto.DataConclusao;

                    _unitOfWork.ObraEtapaRepository.Update(etapaExistente);

                    var etapaNovo = _mapper.Map<ObraEtapa>(etapaExistente);
                    await _auditoriaService.RegistrarAtualizacaoAsync(etapaExistente, etapaNovo, usuarioLogadoId);


                    // Use a coleção de itens que já está em memória e sendo rastreada.
                    // Não consulte o banco de dados novamente aqui.
                    var itensAtuaisDaEtapa = etapaExistente.Itens.ToList();
                    var idsItensDto = etapaDto.Itens.Select(dto => dto.Id).ToHashSet();

                    // Remover itens que não vieram no DTO
                    var itensParaRemover = itensAtuaisDaEtapa.Where(i => !idsItensDto.Contains(i.Id)).ToList();
                    foreach (var item in itensParaRemover)
                    {
                        _unitOfWork.ObraItemEtapaRepository.Remove(item);
                        await _auditoriaService.RegistrarExclusaoAsync(item, usuarioLogadoId);

                    }

                    // Adicionar ou atualizar itens
                    foreach (var itemDto in etapaDto.Itens)
                    {
                        if (itemDto.Id == 0) // Novo item
                        {
                            var novoItem = _mapper.Map<ObraItemEtapa>(itemDto);
                            novoItem.ObraEtapa = etapaExistente;
                            _unitOfWork.ObraItemEtapaRepository.Add(novoItem);

                            await _auditoriaService.RegistrarCriacaoAsync(novoItem, usuarioLogadoId);


                            bool foiConcluidoAgora = novoItem.Concluido;
                            if (foiConcluidoAgora && obraParaNotificacao?.Projeto?.ClienteId != null)
                            {
                                _ = EnviarNotificacaoDeConclusaoAsync(novoItem, etapaExistente.Nome, obraParaNotificacao);
                            }
                        }
                        else // Item existente
                        {
                            // Busca o item na coleção JÁ CARREGADA e rastreada.
                            var itemExistente = itensAtuaisDaEtapa.FirstOrDefault(i => i.Id == itemDto.Id);
                            if (itemExistente != null)
                            {
                                bool eraConcluido = itemExistente.Concluido;
                               var itemNovo = _mapper.Map(itemDto, itemExistente); // Mapeia para o item existente e rastreado
                                _unitOfWork.ObraItemEtapaRepository.Update(itemNovo);
                                await _auditoriaService.RegistrarAtualizacaoAsync(itemExistente, itemNovo, usuarioLogadoId);


                                bool foiConcluidoAgora = itemExistente.Concluido && !eraConcluido;
                                if (foiConcluidoAgora && obraParaNotificacao?.Projeto?.Cliente != null)
                                {
                                    _ = EnviarNotificacaoDeConclusaoAsync(itemExistente, etapaExistente.Nome, obraParaNotificacao);
                                }
                            }
                        }
                    }
                }
            }

            await _unitOfWork.CommitAsync();
        }
       
        private async Task EnviarNotificacaoDeConclusaoAsync(ObraItemEtapa itemConcluido, string nomeEtapa, Obra obra)
        {
            var cliente = obra.Projeto.Cliente;

            if (cliente == null)
            {
                _logger.LogWarning("Não foi possível encontrar o cliente para a obra {ObraId} para enviar a notificação.", obra.Id);
                return;
            }
            var responsavelObra = _unitOfWork.FuncionarioRepository.GetByIdAsync(obra.ResponsavelObraId ?? 0).Result;
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
