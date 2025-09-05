using ByTescaro.ConstrutorApp.Application.DTOs.Relatorios;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.Utils;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class RelatorioObraService : IRelatorioObraService
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper;
        private readonly ILogger<RelatorioObraService> _logger;
        private readonly IPersonalizacaoService _personalizacaoService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
        private readonly ProgressoStrategy _progressStrategy;


        public RelatorioObraService(ILogger<RelatorioObraService> logger, IPersonalizacaoService personalizacaoService, IDbContextFactory<ApplicationDbContext> ctx, IOptions<RelatorioObraOptions> opts)
        {
            _logger = logger;

            QuestPDF.Settings.License = LicenseType.Community;
            _personalizacaoService = personalizacaoService;
            _dbFactory = ctx;
            _progressStrategy = opts?.Value?.ProgressoStrategy ?? ProgressoStrategy.Media;


        }

        public async Task<ObraRelatorioDto?> GetRelatorioAsync(long obraId, CancellationToken ct = default)
        {
            // A) Cabeçalho leve (um contexto dedicado)
            ObraRelatorioDto? head;
            using (var ctx = _dbFactory.CreateDbContext())
            {
                head = await ctx.Set<Obra>()
                    .Where(o => o.Id == obraId)
                    .Select(o => new ObraRelatorioDto
                    {
                        Id = o.Id,
                        NomeObra = o.Nome ?? string.Empty,
                        Status = o.Status.ToString(),
                        DataInicioObra = o.DataInicioExecucao ?? DateTime.MinValue,
                        ClienteNome = o.Projeto.Cliente.Nome ?? string.Empty,
                        ProjetoNome = o.Projeto.Nome ?? string.Empty
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ct);
            }

            if (head == null) return null;

            // Funções locais: cada uma cria e fecha seu próprio DbContext
            async Task<List<ObraEtapaRelatorioDto>> LoadEtapasAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();
                return await ctx.Set<ObraEtapa>()
                    .Where(e => e.ObraId == obraId)                    
                    .Select(e => new ObraEtapaRelatorioDto
                    {
                        Nome = e.Nome ?? string.Empty,
                        //Descricao = e.Descricao,
                        DataInicioPrevista = e.DataInicio ?? default(DateTime),
                        Itens = e.Itens
                            .Select(i => new ObraItemEtapaRelatorioDto
                            {
                                Nome = i.Nome ?? string.Empty,
                                Concluido = i.Concluido
                            }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<ObraInsumoRelatorioDto>> LoadInsumosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();

                // Evita subquery com outro contexto: faz os JOINs aqui
                var query =
                    from oi in ctx.Set<ObraInsumo>()
                    where oi.ObraId == obraId
                    join oil in ctx.Set<ObraInsumoLista>() on oi.ObraInsumoListaId equals oil.Id into oilj
                    from oil in oilj.DefaultIfEmpty()
                    join p in ctx.Set<Pessoa>() on oil.ResponsavelId equals p.Id into pj
                    from p in pj.DefaultIfEmpty()
                    select new ObraInsumoRelatorioDto
                    {
                        InsumoNome = oi.Insumo.Nome ?? string.Empty,
                        UnidadeMedida = oi.Insumo.UnidadeMedida, // atribui enum diretamente
                        Quantidade = oi.Quantidade,
                        IsRecebido = oi.IsRecebido,
                        DataRecebimento = oi.DataRecebimento,
                        ResponsavelRecbimentoNome = p != null ? p.Nome : null
                    };

                return await query.AsNoTracking().ToListAsync(ct);
            }


            async Task<List<ObraServicoRelatorioDto>> LoadServicosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();

                // Evita subquery com outro contexto: faz os JOINs aqui
                var query =
                    from oi in ctx.Set<ObraServico>()
                    where oi.ObraId == obraId
                    join oil in ctx.Set<ObraServicoLista>() on oi.ObraServicoListaId equals oil.Id into oilj
                    from oil in oilj.DefaultIfEmpty()
                    join p in ctx.Set<Pessoa>() on oil.ResponsavelId equals p.Id into pj
                    from p in pj.DefaultIfEmpty()
                    select new ObraServicoRelatorioDto
                    {
                        ServicoNome = oi.Servico.Nome ?? string.Empty,
                        Quantidade = oi.Quantidade,           
                        ResponsavelRecbimentoNome = p != null ? p.Nome : null
                    };

                return await query.AsNoTracking().ToListAsync(ct);
            }

            async Task<List<ObraFuncionarioRelatorioDto>> LoadFuncionariosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();
                return await ctx.Set<ObraFuncionario>()
                    .Where(x => x.ObraId == obraId)
                    .Select(x => new ObraFuncionarioRelatorioDto
                    {
                        FuncionarioNome = x.FuncionarioNome ?? string.Empty,
                        FuncaoNoObra = x.FuncaoNoObra,
                        DataInicio = x.DataInicio,
                        DataFim = x.DataFim
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<ObraEquipamentoRelatorioDto>> LoadEquipamentosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();
                return await ctx.Set<ObraEquipamento>()
                    .Where(x => x.ObraId == obraId)
                    .Select(x => new ObraEquipamentoRelatorioDto
                    {
                        EquipamentoNome = x.EquipamentoNome ?? string.Empty,
                        DataInicioUso = x.DataInicioUso,
                        DataFimUso = x.DataFimUso
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<ObraRetrabalhoRelatorioDto>> LoadRetrabalhosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();

                var query =
                    from r in ctx.Set<ObraRetrabalho>()
                    where r.ObraId == obraId
                    join p in ctx.Set<Pessoa>() on r.ResponsavelId equals p.Id into pj
                    from p in pj.DefaultIfEmpty()
                    select new ObraRetrabalhoRelatorioDto
                    {
                        Descricao = r.Descricao ?? string.Empty,
                        Status = r.Status, // atribui enum diretamente
                        NomeResponsavel = p != null ? p.Nome : null
                    };

                return await query.AsNoTracking().ToListAsync(ct);
            }

            async Task<List<ObraPendenciaRelatorioDto>> LoadPendenciasAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();

                var query =
                    from pnd in ctx.Set<ObraPendencia>()
                    where pnd.ObraId == obraId
                    join p in ctx.Set<Pessoa>() on pnd.ResponsavelId equals p.Id into pj
                    from p in pj.DefaultIfEmpty()
                    select new ObraPendenciaRelatorioDto
                    {
                        Descricao = pnd.Descricao ?? string.Empty,
                        Status = pnd.Status, // atribui enum diretamente
                        NomeResponsavel = p != null ? p.Nome : null
                    };

                return await query.AsNoTracking().ToListAsync(ct);
            }

            async Task<List<ObraDocumentoRelatorioDto>> LoadDocumentosAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();
                return await ctx.Set<ObraDocumento>()
                    .Where(x => x.ObraId == obraId)
                    .Select(x => new ObraDocumentoRelatorioDto
                    {
                        NomeOriginal = x.NomeOriginal ?? string.Empty,
                        Extensao = x.Extensao ?? string.Empty
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            async Task<List<ObraImagemRelatorioDto>> LoadImagensAsync()
            {
                using var ctx = _dbFactory.CreateDbContext();
                return await ctx.Set<ObraImagem>()
                    .Where(x => x.ObraId == obraId)
                    .Select(x => new ObraImagemRelatorioDto
                    {
                        CaminhoRelativo = x.CaminhoRelativo ?? string.Empty
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            // Dispara em paralelo, cada um com seu próprio contexto:
            var etapasTask = LoadEtapasAsync();
            var insumosTask = LoadInsumosAsync();
            var servicosTask = LoadServicosAsync();
            var funcsTask = LoadFuncionariosAsync();
            var equipsTask = LoadEquipamentosAsync();
            var retrabTask = LoadRetrabalhosAsync();
            var pendTask = LoadPendenciasAsync();
            var docsTask = LoadDocumentosAsync();
            var imgsTask = LoadImagensAsync();

            await Task.WhenAll(etapasTask, insumosTask, servicosTask, funcsTask, equipsTask, retrabTask, pendTask, docsTask, imgsTask);

            head.Etapas = etapasTask.Result;
            head.Insumos = insumosTask.Result;
            head.Servicos = servicosTask.Result;
            head.Funcionarios = funcsTask.Result;
            head.Equipamentos = equipsTask.Result;
            head.Retrabalhos = retrabTask.Result;
            head.Pendencias = pendTask.Result;
            head.Documentos = docsTask.Result;
            head.Imagens = imgsTask.Result;

            return head;
        }

        public async Task<byte[]> GerarRelatorioObraPdfAsync(long obraId)
        {
            _logger.LogInformation("Gerando relatório para a Obra ID: {ObraId}", obraId);

            var dto = await GetRelatorioAsync(obraId);
            if (dto == null)
                throw new KeyNotFoundException($"Obra com ID {obraId} não encontrada.");

            // Calcula % por etapa e progresso geral
            foreach (var e in dto.Etapas)
            {
                e.PercentualConclusao = CalcularProgressoEtapa(e);

            }
            dto.ProgressoAtual = _progressStrategy switch
            {
                ProgressoStrategy.PonderadoItens => CalcularProgressoObraPonderado(dto),
                _ => CalcularProgressoObra(dto)
            };

            // Personalização + pré-carrega imagens (sem I/O no compose)
            var personalizacao = await _personalizacaoService.ObterAsync();
            string webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            byte[]? logoBytes = TryLoadBytesSafe(CombineSafe(webRoot, personalizacao?.LogotipoUrl));

            var imagensBytes = dto.Imagens.Select(img =>
            {
                var path = CombineSafe(webRoot, img.CaminhoRelativo);
                return new { img, bytes = TryLoadBytesSafe(path) };
            }).ToList();

            // Compose do PDF (igual ao seu, usando logoBytes e bytes das imagens)
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem(1).AlignLeft().Column(col =>
                        {
                            if (logoBytes != null)
                                col.Item().Height(50).Image(logoBytes).FitArea();
                            else if (!string.IsNullOrEmpty(personalizacao?.LogotipoUrl))
                                col.Item().Text("Logotipo não encontrado").FontSize(8).FontColor(Colors.Red.Medium);
                        });

                        row.RelativeItem(3).AlignCenter().Text(t =>
                        {
                            t.AlignCenter();
                            t.Span($"Relatório da Obra: {dto.NomeObra}").SemiBold().FontSize(18);
                            t.EmptyLine();
                            if (!string.IsNullOrEmpty(personalizacao?.NomeEmpresa))
                                t.Span(personalizacao.NomeEmpresa).SemiBold().FontSize(12);
                        });

                        row.RelativeItem(1);
                    });

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // ——— Informações Gerais ———
                            column.Item().Text("Informações Gerais").SemiBold().FontSize(14);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                });

                                table.Cell().Text("ID da Obra:").SemiBold();
                                table.Cell().Text(dto.Id.ToString());
                                table.Cell().Text("Cliente:").SemiBold();
                                table.Cell().Text(dto.ClienteNome);
                                table.Cell().Text("Projeto:").SemiBold();
                                table.Cell().Text(dto.ProjetoNome);
                                table.Cell().Text("Status:").SemiBold();
                                table.Cell().Text(dto.Status);
                                table.Cell().Text("Data Início:").SemiBold();
                                table.Cell().Text(dto.DataInicioObra.ToShortDateString());
                                table.Cell().Text("Progresso Atual:").SemiBold();
                                table.Cell().Text($"{dto.ProgressoAtual}%");
                            });

                            // ——— Etapas ———
                            if (dto.Etapas.Any())
                            {
                                column.Item().PaddingTop(10).Text("Etapas").SemiBold().FontSize(14);

                                foreach (var etapa in dto.Etapas.OrderBy(e => e.DataInicioPrevista))
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10)
                                        .Column(etapaColumn =>
                                        {
                                            etapaColumn.Spacing(5);
                                            etapaColumn.Item().Text($"{etapa.Nome} ({etapa.PercentualConclusao}%)").SemiBold();
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

                            // ——— Insumos ———
                            if (dto.Insumos.Any())
                            {
                                column.Item().PaddingTop(10).Text("Insumos Utilizados").SemiBold().FontSize(14);

                                var insumosAgrupados = dto.Insumos
                                    .GroupBy(i => i.ResponsavelRecbimentoNome ?? "Não Informado")
                                    .OrderBy(g => g.Key);

                                foreach (var grupo in insumosAgrupados)
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingBottom(10)
                                        .Column(responsavelColumn =>
                                        {
                                            responsavelColumn.Spacing(5);
                                            responsavelColumn.Item().PaddingBottom(5)
                                                .Text($"Recebido por: {grupo.Key}").SemiBold().FontSize(12);

                                            responsavelColumn.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(c =>
                                                {
                                                    c.RelativeColumn(3);
                                                    c.RelativeColumn();
                                                    c.RelativeColumn();
                                                    c.RelativeColumn();
                                                    c.RelativeColumn();
                                                });

                                                table.Header(h =>
                                                {
                                                    h.Cell().BorderBottom(1).Padding(3).Text("Insumo").SemiBold();
                                                    h.Cell().BorderBottom(1).Padding(0).Text("Unidade").SemiBold();
                                                    h.Cell().BorderBottom(1).Padding(0).Text("Qtd").SemiBold();
                                                    h.Cell().BorderBottom(1).Padding(0).Text("Recebido?").SemiBold();
                                                    h.Cell().BorderBottom(1).Padding(0).Text("Recebido em:").SemiBold();
                                                });

                                                foreach (var insumo in grupo.OrderBy(i => i.InsumoNome))
                                                {
                                                    table.Cell().Padding(3).Text(insumo.InsumoNome);
                                                    table.Cell().Padding(0).Text(EnumHelper.ObterDescricaoEnum(insumo.UnidadeMedida));
                                                    table.Cell().Padding(0).Text(insumo.Quantidade.ToString("N2"));
                                                    table.Cell().Padding(0).Text(insumo.IsRecebido ? "Sim" : "Não");
                                                    table.Cell().Padding(0).Text(insumo.DataRecebimento?.ToShortDateString() ?? string.Empty);
                                                }
                                            });
                                        });
                                }
                            }

                            // ——— Servicos ———
                            if (dto.Servicos.Any())
                            {
                                column.Item().PaddingTop(10).Text("Serviços Utilizados").SemiBold().FontSize(14);

                                var servicosAgrupados = dto.Servicos
                                    .GroupBy(i => i.ResponsavelRecbimentoNome ?? "Não Informado")
                                    .OrderBy(g => g.Key);

                                foreach (var grupo in servicosAgrupados)
                                {
                                    column.Item()
                                        .BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingBottom(10)
                                        .Column(responsavelColumn =>
                                        {
                                            responsavelColumn.Spacing(5);
                                            responsavelColumn.Item().PaddingBottom(5)
                                                .Text($"Recebido por: {grupo.Key}").SemiBold().FontSize(12);

                                            responsavelColumn.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(c =>
                                                {
                                                    c.RelativeColumn(3);
                                                    c.RelativeColumn();
                                                    c.RelativeColumn();

                                                });

                                                table.Header(h =>
                                                {
                                                    h.Cell().BorderBottom(1).Padding(3).Text("Servico").SemiBold();
                                                    h.Cell().BorderBottom(1).Padding(0).Text("Qtd").SemiBold();
                                                });

                                                foreach (var servico in grupo.OrderBy(i => i.ServicoNome))
                                                {
                                                    table.Cell().Padding(3).Text(servico.ServicoNome);
                                                    table.Cell().Padding(0).Text(servico.Quantidade.ToString("N2"));
                                                   
                                                }
                                            });
                                        });
                                }
                            }

                            // ——— Funcionários ———
                            if (dto.Funcionarios.Any())
                            {
                                column.Item().PaddingTop(10).Text("Funcionários Alocados").SemiBold().FontSize(14);
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);
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
                                    });

                                    foreach (var f in dto.Funcionarios)
                                    {
                                        table.Cell().Padding(2).Text(f.FuncionarioNome);
                                        table.Cell().Padding(2).Text(f.FuncaoNoObra);
                                        table.Cell().Padding(2).Text(f.DataInicio.ToShortDateString());
                                        table.Cell().Padding(2).Text(f.DataFim?.ToShortDateString() ?? "N/A");
                                    }
                                });
                            }

                            // ——— Equipamentos ———
                            if (dto.Equipamentos.Any())
                            {
                                column.Item().PaddingTop(10).Text("Equipamentos Alocados").SemiBold().FontSize(14);
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
                                        header.Cell().BorderBottom(1).Padding(5).Text("Equipamento").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Início").SemiBold();
                                        header.Cell().BorderBottom(1).Padding(5).Text("Fim").SemiBold();
                                    });

                                    foreach (var e in dto.Equipamentos)
                                    {
                                        table.Cell().Padding(2).Text(e.EquipamentoNome);
                                        table.Cell().Padding(2).Text(e.DataInicioUso.ToShortDateString());
                                        table.Cell().Padding(2).Text(e.DataFimUso?.ToShortDateString() ?? "N/A");
                                    }
                                });
                            }

                            // ——— Retrabalhos ———
                            if (dto.Retrabalhos.Any())
                            {
                                column.Item().PaddingTop(10).Text("Retrabalhos").SemiBold().FontSize(14);
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

                                    foreach (var r in dto.Retrabalhos)
                                    {
                                        table.Cell().Padding(2).Text(r.Descricao);
                                        table.Cell().Padding(2).Text(r.NomeResponsavel);
                                        table.Cell().Padding(2).Text(EnumHelper.ObterDescricaoEnum(r.Status));
                                    }
                                });
                            }

                            // ——— Pendências ———
                            if (dto.Pendencias.Any())
                            {
                                column.Item().PaddingTop(10).Text("Pendências").SemiBold().FontSize(14);
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

                                    foreach (var p in dto.Pendencias)
                                    {
                                        table.Cell().Padding(2).Text(p.Descricao);
                                        table.Cell().Padding(2).Text(p.NomeResponsavel);
                                        table.Cell().Padding(2).Text(EnumHelper.ObterDescricaoEnum(p.Status));
                                    }
                                });
                            }

                            // ——— Documentos ———
                            if (dto.Documentos.Any())
                            {
                                column.Item().PaddingTop(10).Text("Documentos").SemiBold().FontSize(14);
                                foreach (var d in dto.Documentos)
                                    column.Item().Text($"- {d.NomeOriginal} ({d.Extensao})");
                            }

                            // ——— Imagens ———
                            if (dto.Imagens.Any())
                            {
                                column.Item().PaddingTop(10).Text("Imagens").SemiBold().FontSize(14);

                                column.Item().Column(imageContainer =>
                                {
                                    const int colunas = 3;
                                    var blocos = imagensBytes.Chunk(colunas);

                                    foreach (var bloco in blocos)
                                    {
                                        imageContainer.Item().Row(row =>
                                        {
                                            row.Spacing(5);

                                            foreach (var entry in bloco)
                                            {
                                                row.RelativeItem().Column(imgCol =>
                                                {
                                                    if (entry.bytes != null)
                                                    {
                                                        // (2) usa bytes pré-carregados (sem I/O no compose)
                                                        imgCol.Item().Height(100).Width(100).Padding(5)
                                                            .Image(entry.bytes).FitArea();
                                                    }
                                                    else
                                                    {
                                                        imgCol.Item().Height(100).Width(100).Padding(5)
                                                            .AlignCenter().AlignMiddle()
                                                            .Text("Imagem não encontrada").FontSize(8);
                                                    }
                                                });
                                            }

                                            for (int i = bloco.Length; i < colunas; i++)
                                                row.RelativeItem().Text(string.Empty);
                                        });
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

            return await Task.Run(() => document.GeneratePdf());
        }

        private static int CalcularProgressoEtapa(ObraEtapaRelatorioDto etapa)
        {
            if (etapa?.Itens == null || etapa.Itens.Count == 0) return 0;
            int total = etapa.Itens.Count;
            int concluidos = etapa.Itens.Count(i => i.Concluido);
            return (int)Math.Round(100.0 * concluidos / total);
        }

        private static int CalcularProgressoObra(ObraRelatorioDto obra)
        {
            if (obra?.Etapas == null || obra.Etapas.Count == 0) return 0;
            // mesma semântica do seu método antigo: média simples entre etapas
            return (int)Math.Round(obra.Etapas.Average(e => CalcularProgressoEtapa(e)));
        }
        private static string CombineSafe(string root, string? relative)
        {
            relative = (relative ?? string.Empty).Trim().TrimStart('/', '\\');
            return Path.Combine(root, relative);
        }

        private static byte[]? TryLoadBytesSafe(string path)
        {
            try
            {
                if (File.Exists(path))
                    return File.ReadAllBytes(path);
            }
            catch { /* silencioso para não travar render */ }
            return null;
        }

        private static int CalcularProgressoObraPonderado(ObraRelatorioDto obra)
        {
            if (obra?.Etapas == null || obra.Etapas.Count == 0) return 0;

            var itens = obra.Etapas.SelectMany(e => e.Itens ?? Enumerable.Empty<ObraItemEtapaRelatorioDto>()).ToList();
            int total = itens.Count;
            if (total == 0) return 0;

            int concluidos = itens.Count(i => i.Concluido);
            return (int)Math.Round(100.0 * concluidos / total);
        }
    }
}