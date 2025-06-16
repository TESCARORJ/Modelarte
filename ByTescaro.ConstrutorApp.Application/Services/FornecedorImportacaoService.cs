using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ClosedXML.Excel;
using System.Text;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorImportacaoService : IFornecedorImportacaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFornecedorRepository _repo;


        public FornecedorImportacaoService(ApplicationDbContext context, IMapper mapper, IFornecedorRepository repo)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<List<FornecedorDto>> CarregarPreviewAsync(Stream excelStream)
        {
            var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            var fornecedores = new List<FornecedorDto>();

            foreach (var row in rows)
            {
                fornecedores.Add(new FornecedorDto
                {
                    Nome = row.Cell(1).GetString(),
                    TipoPessoa = Enum.TryParse<TipoPessoa>(row.Cell(2).GetString(), out var tipo) ? tipo : null,
                    CpfCnpj = row.Cell(3).GetString(),
                    TelefonePrincipal = row.Cell(4).GetString(),
                    TelefoneWhatsApp = row.Cell(5).GetString(),
                    Email = row.Cell(6).GetString(),
                   
                });
            }

            return fornecedores;
        }

        public async Task<List<ErroImportacaoDto>> ImportarFornecedoresAsync(List<FornecedorDto> fornecedores, string usuario)
        {
            var erros = new List<ErroImportacaoDto>();

            // 🔍 Buscar todos os CPFs já existentes antes do loop
            var cpfsExistentes = (await _repo.GetAllAsync())
                .Select(c => c.CpfCnpj)
                .ToHashSet();

            foreach (var dto in fornecedores)
            {
                if (string.IsNullOrWhiteSpace(dto.CpfCnpj))
                {
                    erros.Add(new ErroImportacaoDto("CPF/CNPJ é obrigatório", dto.Nome));
                    continue;
                }

                if (cpfsExistentes.Contains(dto.CpfCnpj))
                {
                    erros.Add(new ErroImportacaoDto("CPF/CNPJ já cadastrado", dto.CpfCnpj));
                    continue;
                }

                dto.UsuarioCadastro = usuario;
                dto.DataHoraCadastro = DateTime.Now;

                var fornecedor = _mapper.Map<Fornecedor>(dto);
                await _repo.AddAsync(fornecedor); // ✅ await aqui pode ficar, pois é por fornecedor, mas serializado
            }

            await _context.SaveChangesAsync();
            return erros;
        }


    }
}
