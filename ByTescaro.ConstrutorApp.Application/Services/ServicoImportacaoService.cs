using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ClosedXML.Excel;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class ServicoImportacaoService : IServicoImportacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUsuarioLogadoService _usuarioLogadoService;
    private readonly ILogAuditoriaRepository _logRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ServicoImportacaoService(
        ApplicationDbContext context,
        IMapper mapper,
        IUsuarioLogadoService usuarioLogadoService,
        ILogAuditoriaRepository logRepo,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _mapper = mapper;
        _usuarioLogadoService = usuarioLogadoService;
        _logRepo = logRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ServicoDto>> CarregarPreviewAsync(Stream excelStream)
    {
        var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
        var lista = new List<ServicoDto>();

        foreach (var row in rows)
        {
            var nome = row.Cell(1).GetString();
            var descricao = row.Cell(2).GetString();
            var unidadeTexto = row.Cell(3).GetString();

            UnidadeMedida? unidade = null;
            if (!string.IsNullOrWhiteSpace(unidadeTexto))
            {
                // Tenta primeiro pelo nome
                if (Enum.TryParse<UnidadeMedida>(unidadeTexto, ignoreCase: true, out var parsedEnum))
                {
                    unidade = parsedEnum;
                }
                // Tenta pelo número, se o nome falhar
                else if (int.TryParse(unidadeTexto, out var valorNumerico) &&
                         Enum.IsDefined(typeof(UnidadeMedida), valorNumerico))
                {
                    unidade = (UnidadeMedida)valorNumerico;
                }
            }

            lista.Add(new ServicoDto
            {
                Nome = nome,
                Descricao = descricao
            });
        }

        return lista;
    }


    public async Task<List<ErroImportacaoDto>> ImportarServicosAsync(List<ServicoDto> servicos, string usuario)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;
        var usuarioLogadoNome = usuarioLogado?.Nome ?? "Usuário Desconhecido";

        var erros = new List<ErroImportacaoDto>();

        var nomesExistentes = (await _unitOfWork.ServicoRepository.GetAllAsync())
            .Select(i => i.Nome.Trim().ToLower())
            .ToHashSet();

        foreach (var dto in servicos)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                erros.Add(new ErroImportacaoDto("Nome do servico é obrigatório", dto.Nome));
                continue;
            }

            

            if (nomesExistentes.Contains(dto.Nome.Trim().ToLower()))
            {
                erros.Add(new ErroImportacaoDto("Servico já cadastrado", dto.Nome));
                continue;
            }

            dto.UsuarioCadastroId = usuarioLogadoId;
            dto.UsuarioCadastroNome = usuarioLogadoNome;
            dto.DataHoraCadastro = DateTime.Now;
            dto.Ativo = true;

            var entidade = _mapper.Map<Servico>(dto);
            _unitOfWork.ServicoRepository.Add(entidade);
          
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                Entidade = nameof(Servico),
                TipoLogAuditoria = TipoLogAuditoria.Criacao,
                Descricao = $"Serviço {entidade.Nome}  por '{usuarioLogadoNome}' em {DateTime.Now}. ",
                DadosAtuais = JsonSerializer.Serialize(entidade) // Serializa o DTO para o log
            });
        }

        await _unitOfWork.CommitAsync();
        return erros;
    }


}
