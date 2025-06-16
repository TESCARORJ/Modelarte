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
    public class ClienteImportacaoService : IClienteImportacaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IClienteRepository _repo;


        public ClienteImportacaoService(ApplicationDbContext context, IMapper mapper, IClienteRepository repo)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<List<ClienteDto>> CarregarPreviewAsync(Stream excelStream)
        {
            var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            var clientes = new List<ClienteDto>();

            foreach (var row in rows)
            {
                clientes.Add(new ClienteDto
                {
                    Nome = row.Cell(1).GetString(),
                    TipoPessoa = Enum.TryParse<TipoPessoa>(row.Cell(2).GetString(), out var tipo) ? tipo : null,
                    CpfCnpj = row.Cell(3).GetString(),
                    TelefonePrincipal = row.Cell(4).GetString(),
                    TelefoneWhatsApp = row.Cell(5).GetString(),
                    Email = row.Cell(6).GetString(),
                    CEP = row.Cell(7).GetString(),
                    Logradouro = row.Cell(8).GetString(),
                    Numero = row.Cell(9).GetString(),
                    Complemento = row.Cell(10).GetString(),
                    Bairro = row.Cell(11).GetString(),
                    Cidade = row.Cell(12).GetString(),
                    Estado = row.Cell(13).GetString(),
                    UF = row.Cell(14).GetString()
                });
            }

            return clientes;
        }

        public async Task<List<ErroImportacaoDto>> ImportarClientesAsync(List<ClienteDto> clientes, string usuario)
        {
            var erros = new List<ErroImportacaoDto>();

            // 🔍 Buscar todos os CPFs já existentes antes do loop
            var cpfsExistentes = (await _repo.GetAllAsync())
                .Select(c => c.CpfCnpj)
                .ToHashSet();

            foreach (var dto in clientes)
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

                var cliente = _mapper.Map<Cliente>(dto);
                await _repo.AddAsync(cliente); // ✅ await aqui pode ficar, pois é por cliente, mas serializado
            }

            await _context.SaveChangesAsync();
            return erros;
        }


    }
}
