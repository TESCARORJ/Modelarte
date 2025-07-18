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

                dto.UsuarioCadastroId = usuarioLogadoId;
                dto.UsuarioCadastroNome = usuarioLogadoNome;
                dto.DataHoraCadastro = DateTime.Now;

                var fornecedor = _mapper.Map<Fornecedor>(dto);
                _unitOfWork.FornecedorRepository.Add(fornecedor);
               
                await _logRepo.RegistrarAsync(new LogAuditoria
                {
                    UsuarioId = usuarioLogado == null ? 0 : usuarioLogado.Id,
                    UsuarioNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome,
                    Entidade = nameof(Fornecedor),
                    TipoLogAuditoria = TipoLogAuditoria.Criacao,
                    Descricao = $"Fornecedor {fornecedor.Nome} importado por '{usuarioLogado}' em {DateTime.Now}. ",
                    DadosAtuais = JsonSerializer.Serialize(fornecedor) // Serializa o DTO para o log
                });
            }

            await _unitOfWork.CommitAsync();
            return erros;
        }


    }
}
