using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ClosedXML.Excel;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Services;

public class InsumoImportacaoService : IInsumoImportacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IInsumoRepository _repo;

    public InsumoImportacaoService(ApplicationDbContext context, IMapper mapper, IInsumoRepository repo)
    {
        _context = context;
        _mapper = mapper;
        _repo = repo;
    }

    public async Task<List<InsumoDto>> CarregarPreviewAsync(Stream excelStream)
    {
        var workbook = new XLWorkbook(excelStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
        var lista = new List<InsumoDto>();

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

            lista.Add(new InsumoDto
            {
                Nome = nome,
                Descricao = descricao,
                UnidadeMedida = unidade
            });
        }

        return lista;
    }


    public async Task<List<ErroImportacaoDto>> ImportarInsumosAsync(List<InsumoDto> insumos, string usuario)
    {
        var erros = new List<ErroImportacaoDto>();

        var nomesExistentes = (await _repo.GetAllAsync())
            .Select(i => i.Nome.Trim().ToLower())
            .ToHashSet();

        foreach (var dto in insumos)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                erros.Add(new ErroImportacaoDto("Nome do insumo é obrigatório", dto.Nome));
                continue;
            }

            if (dto.UnidadeMedida == null)
            {
                erros.Add(new ErroImportacaoDto("Unidade de medida é obrigatória", dto.Nome));
                continue;
            }

            if (nomesExistentes.Contains(dto.Nome.Trim().ToLower()))
            {
                erros.Add(new ErroImportacaoDto("Insumo já cadastrado", dto.Nome));
                continue;
            }

            dto.UsuarioCadastro = usuario;
            dto.DataHoraCadastro = DateTime.Now;
            dto.Ativo = true;

            var entidade = _mapper.Map<Insumo>(dto);
            _repo.Add(entidade);
        }

        await _context.SaveChangesAsync();
        return erros;
    }


}
