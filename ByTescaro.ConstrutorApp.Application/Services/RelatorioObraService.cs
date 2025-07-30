using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs.Relatorios;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Utils; // Adicionado para Path.Combine e File.Exists
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class RelatorioObraService : IRelatorioObraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RelatorioObraService> _logger;

        public RelatorioObraService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RelatorioObraService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;

            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GerarRelatorioObraPdfAsync(long obraId)
        {
            _logger.LogInformation($"Gerando relatório para a Obra ID: {obraId}");

            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(obraId);

            if (obra == null)
            {
                _logger.LogWarning($"Obra com ID {obraId} não encontrada para geração de relatório.");
                throw new KeyNotFoundException($"Obra com ID {obraId} não encontrada.");
            }

            var obraRelatorioDto = _mapper.Map<ObraRelatorioDto>(obra);

            obraRelatorioDto.ProgressoAtual = CalcularProgressoObra(obra);
            //obraRelatorioDto.OrcamentoTotal = CalcularOrcamentoTotalObra(obra); // Certifique-se de que este método está descomentado e funcional.

            foreach (var etapa in obraRelatorioDto.Etapas)
            {
                // Aqui, a propriedade Nome da Etapa (Domain) deve ser mapeada para Nome (DTO)
                // Se o nome da propriedade na entidade for 'NomeEtapa', o mapeamento deve ser:
                // CreateMap<ObraEtapa, ObraEtapaRelatorioDto>().ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeEtapa));
                // Caso contrário, se for apenas 'Nome', a linha abaixo está correta.
                var etapaOriginal = obra.Etapas.FirstOrDefault(e => e.Nome == etapa.Nome); // Assumindo que ObraEtapa tem NomeEtapa
                if (etapaOriginal != null)
                {
                    etapa.PercentualConclusao = CalcularProgressoEtapa(etapaOriginal);
                }
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .PaddingBottom(10)
                        .Text($"Relatório da Obra: {obraRelatorioDto.NomeObra}")
                        .SemiBold().FontSize(18).AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Text("Informações Gerais").SemiBold().FontSize(14);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                });

                                table.Cell().Text("ID da Obra:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.Id.ToString());
                                table.Cell().Text("Cliente:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.ClienteNome);
                                table.Cell().Text("Projeto:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.ProjetoNome);
                                table.Cell().Text("Status:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.Status);
                                table.Cell().Text("Data Início:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.DataInicioObra.ToShortDateString());
                                table.Cell().Text("Conclusão Prevista:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.DataConclusaoPrevista?.ToShortDateString() ?? "N/A");
                                table.Cell().Text("Progresso Atual:").SemiBold();
                                table.Cell().Text($"{obraRelatorioDto.ProgressoAtual}%");
                                //table.Cell().Text("Orçamento Total:").SemiBold();
                                //table.Cell().Text(obraRelatorioDto.OrcamentoTotal.ToString("C"));
                                //table.Cell().Text("Descrição:").SemiBold();
                                //table.Cell().Text(obraRelatorioDto.Descricao);
                            });

                            if (obraRelatorioDto.Etapas.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Etapas").SemiBold().FontSize(14);

                                foreach (var etapa in obraRelatorioDto.Etapas.OrderBy(e => e.DataInicioPrevista))
                                {
                                    // APLICAR ESTILOS AO CONTAINER DO ITEM, NÃO AO RESULTADO DA DEFINIÇÃO DA COLUNA INTERNA
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10) // <-- CORREÇÃO AQUI
                                        .Column(etapaColumn =>
                                        {
                                            etapaColumn.Spacing(5);
                                            etapaColumn.Item().Text($"{etapa.Nome} ({etapa.PercentualConclusao}%)").SemiBold();
                                            etapaColumn.Item().Text($"Período: {etapa.DataInicioPrevista.ToShortDateString()} - {etapa.DataConclusaoPrevista?.ToShortDateString() ?? "N/A"}");
                                            etapaColumn.Item().Text(etapa.Descricao);

                                            if (etapa.Itens.Any())
                                            {
                                                etapaColumn.Item().PaddingLeft(10).PaddingBottom(5)
                                                    .Text("Itens da Etapa:").FontSize(11);

                                                foreach (var item in etapa.Itens)
                                                {
                                                    etapaColumn.Item().PaddingLeft(20)
                                                        .Text($"- {(item.Concluido ? "[X]" : "[ ]")} {item.Nome}")
                                                        .FontSize(10);
                                                }
                                            }
                                        }); // Removidos os estilos daqui
                                }
                            }

                            if (obraRelatorioDto.Insumos.Any()) // ObraRelatorioDto.Insumos é novamente uma lista plana
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Insumos Utilizados").SemiBold().FontSize(14);

                                var insumosAgrupados = obraRelatorioDto.Insumos
                                    .GroupBy(i => i.ResponsavelRecbimentoNome ?? "Não Informado") // Agrupa por responsável
                                    .OrderBy(g => g.Key);

                                foreach (var grupoResponsavel in insumosAgrupados)
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingBottom(10) // Estilos no item que contém a coluna
                                        .Column(responsavelColumn =>
                                        {
                                            responsavelColumn.Spacing(5);
                                            responsavelColumn.Item().PaddingBottom(5)
                                                .Text($"Recebido por: {grupoResponsavel.Key}") // Nome do responsável
                                                .SemiBold().FontSize(12);

                                            responsavelColumn.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(columns =>
                                                {
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                });

                                                table.Header(header =>
                                                {
                                                    header.Cell().Text("Insumo").SemiBold();
                                                    header.Cell().Text("Unidade").SemiBold();
                                                    header.Cell().Text("Qtd").SemiBold();
                                                    header.Cell().Text("Recebido?").SemiBold();
                                                    header.Cell().Text("Recebido em:").SemiBold();
                                                });

                                                foreach (var insumo in grupoResponsavel.OrderBy(i => i.InsumoNome)) // Ordenar insumos dentro do grupo
                                                {
                                                    table.Cell().Text(insumo.InsumoNome);
                                                    table.Cell().Text(EnumHelper.ObterDescricaoEnum(insumo.UnidadeMedida));
                                                    table.Cell().Text(insumo.Quantidade.ToString("N2"));
                                                    table.Cell().Text(insumo.IsRecebido ? "Sim" : "Não");
                                                    table.Cell().Text(insumo.DataRecebimento?.ToShortDateString() ?? string.Empty);
                                                }
                                            });
                                        });
                                }
                            }

                            if (obraRelatorioDto.Funcionarios.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Funcionários Alocados").SemiBold().FontSize(14);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).Text("Funcionário").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Função").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Início").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Fim").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Custo Diário").SemiBold();
                                    });

                                    foreach (var func in obraRelatorioDto.Funcionarios)
                                    {
                                        table.Cell().Padding(2).Text(func.NomeFuncionario);
                                        table.Cell().Padding(2).Text(func.Funcao);
                                        table.Cell().Padding(2).Text(func.DataInicioAlocacao.ToShortDateString());
                                        table.Cell().Padding(2).Text(func.DataFimAlocacao?.ToShortDateString() ?? "N/A");
                                        table.Cell().Padding(2).AlignRight().Text(func.CustoDiario.ToString("C"));
                                    }
                                });
                            }

                            if (obraRelatorioDto.Equipamentos.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Equipamentos Alocados").SemiBold().FontSize(14);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).Text("Equipamento").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Status").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Início").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Fim").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Custo Diário").SemiBold();
                                    });

                                    foreach (var equip in obraRelatorioDto.Equipamentos)
                                    {
                                        table.Cell().Padding(2).Text(equip.NomeEquipamento);
                                        table.Cell().Padding(2).Text(equip.Status);
                                        table.Cell().Padding(2).Text(equip.DataInicioAlocacao.ToShortDateString());
                                        table.Cell().Padding(2).Text(equip.DataFimAlocacao?.ToShortDateString() ?? "N/A");
                                        table.Cell().Padding(2).AlignRight().Text(equip.CustoDiario.ToString("C"));
                                    }
                                });
                            }

                            if (obraRelatorioDto.Documentos.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Documentos").SemiBold().FontSize(14);
                                foreach (var doc in obraRelatorioDto.Documentos)
                                {
                                    column.Item().Text($"- {doc.NomeDocumento} ({doc.TipoArquivo})");
                                }
                            }

                            if (obraRelatorioDto.Imagens.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Imagens").SemiBold().FontSize(14);
                                column.Item().Grid(grid =>
                                {
                                    grid.Columns(3); // 3 imagens por linha
                                    foreach (var img in obraRelatorioDto.Imagens)
                                    {
                                        try
                                        {
                                            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.CaminhoImagem.TrimStart('/'));
                                            if (System.IO.File.Exists(imagePath))
                                            {
                                                grid.Item().Height(100).Width(100).Padding(5)
                                                    .Image(imagePath).FitArea();
                                            }
                                            else
                                            {
                                                _logger.LogWarning($"Imagem não encontrada no caminho: {imagePath}");
                                                grid.Item().Height(100).Width(100).Padding(5)
                                                    .Text("Imagem não encontrada").FontSize(8).AlignCenter();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, $"Erro ao carregar imagem para o relatório: {img.CaminhoImagem}");
                                            grid.Item().Height(100).Width(100).Padding(5)
                                                .Text("Erro ao carregar imagem").FontSize(8).AlignCenter();
                                        }
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Gerado em: ").FontSize(8);
                            x.Span($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}").FontSize(8);
                            x.Span(" | Página ").FontSize(8);
                            x.CurrentPageNumber().FontSize(8);
                            x.Span(" de ").FontSize(8);
                            x.TotalPages().FontSize(8);
                        });
                });
            });

            return document.GeneratePdf();
        }

        // Métodos auxiliares (podem ser movidos para um utilitário se usados em mais lugares)
        private int CalcularProgressoObra(Obra obra)
        {
            if (obra == null || !obra.Etapas.Any())
                return 0;

            var totalEtapas = obra.Etapas.Count;
            if (totalEtapas == 0) return 0;

            int progressoTotalEtapas = 0;
            foreach (var etapa in obra.Etapas)
            {
                progressoTotalEtapas += CalcularProgressoEtapa(etapa);
            }

            return (int)Math.Round((double)progressoTotalEtapas / totalEtapas);
        }

        private int CalcularProgressoEtapa(ObraEtapa etapa)
        {
            if (etapa == null || !etapa.Itens.Any())
                return 0;

            var totalItens = etapa.Itens.Count;
            if (totalItens == 0) return 0;

            var itensConcluidos = etapa.Itens.Count(item => item.Concluido);
            return (int)Math.Round((double)itensConcluidos / totalItens * 100);
        }

        //private decimal CalcularOrcamentoTotalObra(Obra obra)
        //{
        //    if (obra?.Orcamentos == null || !obra.Orcamentos.Any())
        //        return 0;

        //    return obra.Orcamentos.Sum(o => o.ValorTotal);
        //}
    }
}