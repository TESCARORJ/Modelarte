using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using System.Text;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FornecedorImportacaoService : IFornecedorImportacaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly ILogAuditoriaRepository _logRepo;



        public FornecedorImportacaoService(IUnitOfWork unitOfWork, IMapper mapper, IAuditoriaService auditoriaService, IUsuarioLogadoService usuarioLogadoService, ILogAuditoriaRepository logRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
            _logRepo = logRepo;
        }

        public async Task<List<FornecedorDto>> CarregarPreviewAsync(Stream excelStream)
        {
            var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheets.First();
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            var fornecedores = new List<FornecedorDto>();



            foreach (var row in rows)
            {
                var nome = row.Cell(1).GetString();
                var tipoPessoaTexto = row.Cell(2).GetString();
                var cpfCnpj = row.Cell(3).GetString();
                var telefonePrincipal = row.Cell(4).GetString();
                var telefoneWhatsApp = row.Cell(5).GetString();
                var email = row.Cell(6).GetString();
                var tipoFornecedorTexto = row.Cell(7).GetString();
                var cep = row.Cell(8).GetString();
                var logradouro = row.Cell(9).GetString();
                var numero = row.Cell(10).GetString();
                var complemento = row.Cell(11).GetString();
                var bairro = row.Cell(12).GetString();
                var cidade = row.Cell(13).GetString();
                var estado = row.Cell(14).GetString();
                var uf = row.Cell(15).GetString();


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


                TipoFornecedor? tipoFornecedor = null;
                if (!string.IsNullOrWhiteSpace(tipoFornecedorTexto))
                {
                    // Tenta primeiro pelo nome
                    if (Enum.TryParse<TipoFornecedor>(tipoFornecedorTexto, ignoreCase: true, out var parsedEnum))
                    {
                        tipoFornecedor = parsedEnum;
                    }
                    // Tenta pelo número, se o nome falhar
                    else if (int.TryParse(tipoFornecedorTexto, out var valorNumerico) &&
                             Enum.IsDefined(typeof(UnidadeMedida), valorNumerico))
                    {
                        tipoFornecedor = (TipoFornecedor)valorNumerico;
                    }
                }

                fornecedores.Add(new FornecedorDto
                {
                    Nome = nome,
                    TipoPessoa = tipoPessoa,
                    CpfCnpj = cpfCnpj,
                    TelefonePrincipal = telefonePrincipal,
                    TelefoneWhatsApp = telefoneWhatsApp,
                    TipoFornecedor = tipoFornecedor,
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

            return fornecedores;
        }

        public async Task<List<ErroImportacaoDto>> ImportarFornecedoresAsync(List<FornecedorDto> fornecedores, string usuario)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var usuarioLogadoNome = usuarioLogado?.Nome ?? "Usuário Desconhecido";

            var erros = new List<ErroImportacaoDto>();

            // 🔍 Buscar todos os CPFs já existentes antes do loop
            var cpfsExistentes = (await _unitOfWork.FornecedorRepository.GetAllAsync())
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

                if (dto.TipoPessoa == null || (int)dto.TipoPessoa == 0 )
                {
                    erros.Add(new ErroImportacaoDto("Selecione o Tipo de Pessoa", dto.CpfCnpj));
                    continue;
                }
                if (dto.TipoFornecedor == null || (int)dto.TipoFornecedor == 0 )
                {
                    erros.Add(new ErroImportacaoDto("Selecione o Tipo de Fornecedor", dto.CpfCnpj));
                    continue;
                }

                dto.UsuarioCadastroId = usuarioLogadoId;
                dto.UsuarioCadastroNome = usuarioLogadoNome;
                dto.DataHoraCadastro = DateTime.Now;
                dto.Ativo = true;
               

                var fornecedor = _mapper.Map<Fornecedor>(dto);

                fornecedor.TipoEntidade = TipoEntidadePessoa.Fornecedor;

                _unitOfWork.FornecedorRepository.Add(fornecedor);
               
                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                    UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                    Entidade = nameof(Fornecedor),
                    TipoLogAuditoria = TipoLogAuditoria.Criacao,
                    Descricao = $"Fornecedor {fornecedor.Nome} importado por '{usuarioLogadoNome}' em {DateTime.Now}. ",
                    DadosAtuais = JsonSerializer.Serialize(fornecedor) // Serializa o DTO para o log
                });
            }

            await _unitOfWork.CommitAsync();
            return erros;
        }


    }
}
