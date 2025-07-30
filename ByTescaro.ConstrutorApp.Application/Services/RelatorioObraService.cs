using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs.Relatorios;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Utils; // Para EnumHelper
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            //obraRelatorioDto.OrcamentoTotal = CalcularOrcamentoTotalObra(obra);

            foreach (var etapa in obraRelatorioDto.Etapas)
            {
                var etapaOriginal = obra.Etapas.FirstOrDefault(e => e.Nome == etapa.Nome);
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
                                table.Cell().Text("Orçamento Total:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.OrcamentoTotal.ToString("C"));
                                table.Cell().Text("Descrição:").SemiBold();
                                table.Cell().Text(obraRelatorioDto.Descricao);
                            });

                            if (obraRelatorioDto.Etapas.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Etapas").SemiBold().FontSize(14);

                                foreach (var etapa in obraRelatorioDto.Etapas.OrderBy(e => e.DataInicioPrevista))
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10)
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
                                        });
                                }
                            }

                            if (obraRelatorioDto.Insumos.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Insumos Utilizados").SemiBold().FontSize(14);

                                var insumosAgrupados = obraRelatorioDto.Insumos
                                    .GroupBy(i => i.ResponsavelRecbimentoNome ?? "Não Informado")
                                    .OrderBy(g => g.Key);

                                foreach (var grupoResponsavel in insumosAgrupados)
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingBottom(10)
                                        .Column(responsavelColumn =>
                                        {
                                            responsavelColumn.Spacing(5);
                                            responsavelColumn.Item().PaddingBottom(5).Text($"Recebido por: {grupoResponsavel.Key}")
                                                .SemiBold().FontSize(12);

                                            responsavelColumn.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(columns =>
                                                {
                                                    columns.RelativeColumn(3);
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                    columns.RelativeColumn();
                                                });

                                                table.Header(header =>
                                                {
                                                    header.Cell().BorderBottom(1).Padding(5).Text("Insumo").SemiBold();
                                                    header.Cell().BorderBottom(1).Padding(5).Text("Unidade").SemiBold();
                                                    header.Cell().BorderBottom(1).Padding(5).Text("Qtd").SemiBold();
                                                    header.Cell().BorderBottom(1).Padding(5).Text("Recebido?").SemiBold();
                                                    header.Cell().BorderBottom(1).Padding(5).Text("Recebido em:").SemiBold();
                                                });

                                                foreach (var insumo in grupoResponsavel.OrderBy(i => i.InsumoNome))
                                                {
                                                    table.Cell().Padding(2).Text(insumo.InsumoNome);
                                                    table.Cell().Padding(2).Text(EnumHelper.ObterDescricaoEnum(insumo.UnidadeMedida));
                                                    table.Cell().Padding(2).AlignRight().Text(insumo.Quantidade.ToString("N2"));
                                                    table.Cell().Padding(2).AlignRight().Text(insumo.IsRecebido ? "Sim" : "Não");
                                                    table.Cell().Padding(2).AlignRight().Text(insumo.DataRecebimento?.ToShortDateString() ?? string.Empty);
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
                            
                            if (obraRelatorioDto.Retrabalhos.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Retrabalhos").SemiBold().FontSize(14);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).Text("Descrição").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Responsável").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Status").SemiBold();
                                    });

                                    foreach (var retrab in obraRelatorioDto.Retrabalhos)
                                    {
                                        table.Cell().Padding(2).Text(retrab.Descricao);
                                        table.Cell().Padding(2).Text(retrab.NomeResponsavel);
                                        table.Cell().Padding(2).Text(EnumHelper.ObterDescricaoEnum(retrab.Status));
                                    }
                                });
                            }

                            if (obraRelatorioDto.Pendencias.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Pendências").SemiBold().FontSize(14);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();

                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).Text("Descrição").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Responsável").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Status").SemiBold();
                                    });

                                    foreach (var pend in obraRelatorioDto.Pendencias)
                                    {
                                        table.Cell().Padding(2).Text(pend.Descricao);
                                        table.Cell().Padding(2).Text(pend.NomeResponsavel);
                                        table.Cell().Padding(2).Text(EnumHelper.ObterDescricaoEnum(pend.Status));
                                    }
                                });
                            }

                            if (obraRelatorioDto.Documentos.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Documentos").SemiBold().FontSize(14);
                                foreach (var doc in obraRelatorioDto.Documentos)
                                {
                                    column.Item().Text($"- {doc.NomeOriginal} ({doc.Extensao})");
                                }
                            }

                            if (obraRelatorioDto.Imagens.Any())
                            {
                                column.Item().PaddingTop(10)
                                    .Text("Imagens").SemiBold().FontSize(14);

                                // Substituído Grid por Column e Row
                                column.Item().Column(imageContainer =>
                                {
                                    const int colunas = 3; // Número de colunas desejado
                                    var imagens = obraRelatorioDto.Imagens.ToList();

                                    // Iterar pelas imagens em blocos de 'colunas'
                                    for (int i = 0; i < imagens.Count; i += colunas)
                                    {
                                        var imagensDaLinha = imagens.Skip(i).Take(colunas).ToList();

                                        imageContainer.Item().Row(row =>
                                        {
                                            row.Spacing(5); // Espaçamento entre as imagens na linha

                                            foreach (var img in imagensDaLinha)
                                            {
                                                row.RelativeItem().Column(imgCol => // Usar Column dentro de Row para cada imagem
                                                {
                                                    try
                                                    {
                                                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.CaminhoRelativo.TrimStart('/'));
                                                        if (System.IO.File.Exists(imagePath))
                                                        {
                                                            imgCol.Item().Height(100).Width(100).Padding(5)
                                                                .Image(imagePath).FitArea();
                                                        }
                                                        else
                                                        {
                                                            _logger.LogWarning($"Imagem não encontrada no caminho: {imagePath}");
                                                            imgCol.Item().Height(100).Width(100).Padding(5)
                                                                .AlignCenter().AlignMiddle()
                                                                .Text("Imagem não encontrada").FontSize(8);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _logger.LogError(ex, $"Erro ao carregar imagem para o relatório: {img.CaminhoRelativo}");
                                                        imgCol.Item().Height(100).Width(100).Padding(5)
                                                            .AlignCenter().AlignMiddle()
                                                            .Text("Erro ao carregar imagem").FontSize(8);
                                                    }
                                                });
                                            }

                                            // Adicionar itens vazios para preencher a linha se houver menos imagens que o número de colunas
                                            int colunasFaltantes = colunas - imagensDaLinha.Count;
                                            for (int j = 0; j < colunasFaltantes; j++)
                                            {
                                                row.RelativeItem().Text(string.Empty); // Adiciona um espaço vazio
                                            }
                                        });

                                        // Adicionar um pequeno padding entre as linhas de imagens
                                        if (i + colunas < imagens.Count)
                                        {
                                            imageContainer.Item().PaddingBottom(5).Text(string.Empty);
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