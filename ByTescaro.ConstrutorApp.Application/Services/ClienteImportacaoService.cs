using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ClienteImportacaoService : IClienteImportacaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IClienteRepository _repo;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ClienteImportacaoService(ApplicationDbContext context, IMapper mapper, IClienteRepository repo, ILogAuditoriaRepository logRepo, IUsuarioLogadoService usuarioLogadoService)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
            _logRepo = logRepo;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<List<ClienteDto>> CarregarPreviewAsync(Stream excelStream)
        {
            var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            var clientes = new List<ClienteDto>();



            foreach (var row in rows)
            {
                var nome = row.Cell(1).GetString();
                var tipoPessoaTexto = row.Cell(2).GetString();
                var cpfCnpj = row.Cell(3).GetString();
                var telefonePrincipal = row.Cell(4).GetString();
                var telefoneWhatsApp = row.Cell(5).GetString();
                var email = row.Cell(6).GetString();
                var cep = row.Cell(7).GetString();
                var logradouro = row.Cell(8).GetString();
                var numero = row.Cell(9).GetString();
                var complemento = row.Cell(10).GetString();
                var bairro = row.Cell(11).GetString();
                var cidade = row.Cell(12).GetString();
                var estado = row.Cell(13).GetString();
                var uf = row.Cell(14).GetString();


                TipoPessoa? tipoPessoa = null;
                if (!string.IsNullOrWhiteSpace(tipoPessoaTexto))
                {
                    // Tenta primeiro pelo nome
                    if (Enum.TryParse<TipoPessoa>(tipoPessoaTexto, ignoreCase: true, out var parsedEnum))
                    {
                        tipoPessoa = parsedEnum;
                    }
                    // Tenta pelo número, se o nome falhar
                    else if (int.TryParse(tipoPessoaTexto, out var valorNumerico) &&
                             Enum.IsDefined(typeof(UnidadeMedida), valorNumerico))
                    {
                        tipoPessoa = (TipoPessoa)valorNumerico;
                    }
                }

                clientes.Add(new ClienteDto
                {
                    Nome = nome,
                    TipoPessoa = tipoPessoa,
                    CpfCnpj = cpfCnpj,
                    TelefonePrincipal = telefonePrincipal,
                    TelefoneWhatsApp = telefoneWhatsApp,
                    Email = email,
                    Ativo = true,
                    CEP = cep,
                    Logradouro = logradouro,
                    Numero = numero,
                    Complemento = complemento,
                    Bairro = bairro,
                    Cidade = cidade,
                    Estado = estado,
                    UF = uf
                });
            }

            return clientes;
        }

        public async Task<List<ErroImportacaoDto>> ImportarClientesAsync(List<ClienteDto> clientes, string usuario)
        {
            var erros = new List<ErroImportacaoDto>();
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;
            var usuarioLogadoNome = usuarioLogado?.Nome ?? "Usuário Desconhecido";


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

                if (dto.TipoPessoa == null || (int)dto.TipoPessoa == 0)
                {
                    erros.Add(new ErroImportacaoDto("Selecione o Tipo de Pessoa", dto.CpfCnpj));
                    continue;
                }

                dto.UsuarioCadastroId = usuarioLogadoId;
                dto.UsuarioCadastroNome = usuarioLogadoNome;
                dto.DataHoraCadastro = DateTime.Now;
                dto.Ativo = true;

                var cliente = _mapper.Map<Cliente>(dto);

                cliente.TipoEntidade = TipoEntidadePessoa.Cliente;

                _repo.Add(cliente);

                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                    UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                    Entidade = nameof(Cliente),
                    TipoLogAuditoria = TipoLogAuditoria.Criacao,
                    Descricao = $"Cliente '{cliente.Nome}' importado por {usuarioLogadoNome} em {DateTime.Now}",
                    DadosAtuais = JsonSerializer.Serialize(dto) // Serializa o DTO para o log
                });
            }

            await _context.SaveChangesAsync();
            return erros;
        }


    }
}
