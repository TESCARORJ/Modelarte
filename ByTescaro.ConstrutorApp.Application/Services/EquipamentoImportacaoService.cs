using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ClosedXML.Excel;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class EquipamentoImportacaoService : IEquipamentoImportacaoService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogAuditoriaRepository _logRepo;
    private readonly IUsuarioLogadoService _usuarioLogadoService;

    public EquipamentoImportacaoService(IMapper mapper, ILogAuditoriaRepository logRepo, IUsuarioLogadoService usuarioLogadoService, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logRepo = logRepo;
        _usuarioLogadoService = usuarioLogadoService;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EquipamentoDto>> CarregarPreviewAsync(Stream excelStream)
    {
        var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
        var lista = new List<EquipamentoDto>();

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

            lista.Add(new EquipamentoDto
            {
                Nome = nome,
                Descricao = descricao,
            });
        }

        return lista;
    }


    public async Task<List<ErroImportacaoDto>> ImportarEquipamentosAsync(List<EquipamentoDto> equipamentos, string usuario)
    {
        var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
        var usuarioLogadoId = usuarioLogado?.Id ?? 0;
        var usuarioLogadoNome = usuarioLogado?.Nome ?? "Usuário Desconhecido";

        var erros = new List<ErroImportacaoDto>();

        var nomesExistentes = (await _unitOfWork.EquipamentoRepository.GetAllAsync())
            .Select(i => i.Nome.Trim().ToLower())
            .ToHashSet();

        foreach (var dto in equipamentos)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                erros.Add(new ErroImportacaoDto("Nome do equipamento é obrigatório", dto.Nome));
                continue;
            }


            if (nomesExistentes.Contains(dto.Nome.Trim().ToLower()))
            {
                erros.Add(new ErroImportacaoDto("Equipamento já cadastrado", dto.Nome));
                continue;
            }

            dto.UsuarioCadastroId = usuarioLogadoId;
            dto.UsuarioCadastroNome = usuarioLogadoNome;
            dto.DataHoraCadastro = DateTime.Now;
            dto.Ativo = true;

            var entidade = _mapper.Map<Equipamento>(dto);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                Entidade = nameof(Equipamento),
                TipoLogAuditoria = TipoLogAuditoria.Criacao,
                Descricao = $"Equipamento {entidade.Nome} importado por '{usuarioLogado}' em {DateTime.Now}. ",
                DadosAtuais = JsonSerializer.Serialize(entidade) // Serializa o DTO para o log
            });

            _unitOfWork.EquipamentoRepository.Add(entidade);
        }

        await _unitOfWork.CommitAsync();
        return erros;
    }


}
