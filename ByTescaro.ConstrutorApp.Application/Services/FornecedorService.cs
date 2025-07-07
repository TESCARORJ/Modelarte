using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class FornecedorService : IFornecedorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FornecedorService(
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";

        public async Task<IEnumerable<FornecedorDto>> ObterTodosAsync()
        {
            var fornecedores = await _unitOfWork.FornecedorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FornecedorDto>>(fornecedores);
        }

        public async Task<FornecedorDto?> ObterPorIdAsync(long id)
        {
            var fornecedor = await _unitOfWork.FornecedorRepository.GetByIdAsync(id);
            return fornecedor == null ? null : _mapper.Map<FornecedorDto>(fornecedor);
        }

        public async Task CriarAsync(FornecedorDto dto)
        {
            var entity = _mapper.Map<Fornecedor>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastro = UsuarioLogado;

            entity.TipoEntidade = TipoEntidadePessoa.Fornecedor;
            _unitOfWork.FornecedorRepository.Add(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Fornecedor),
                Acao = "Criado",
                Descricao = $"Fornecedor '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(FornecedorDto dto)
        {
            var entityAntigo = await _unitOfWork.FornecedorRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map<Fornecedor>(dto);
            entityNovo.UsuarioCadastro = entityAntigo.UsuarioCadastro;
            entityNovo.DataHoraCadastro = entityAntigo.DataHoraCadastro;

            entityNovo.TipoEntidade = TipoEntidadePessoa.Fornecedor;
            _unitOfWork.FornecedorRepository.Update(entityNovo);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Fornecedor),
                Acao = "Atualizado",
                Descricao = $"Fornecedor '{entityNovo.Nome}' atualizado",
                DadosAnteriores = JsonSerializer.Serialize(entityAntigo),
                DadosAtuais = JsonSerializer.Serialize(entityNovo)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.FornecedorRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.FornecedorRepository.Remove(entity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Fornecedor),
                Acao = "Excluído",
                Descricao = $"Fornecedor '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });

            await _unitOfWork.CommitAsync();

        }

        public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
            {
                return false;
            }

            return await _unitOfWork.FornecedorRepository.ExistsAsync(f => f.TipoEntidade ==TipoEntidadePessoa.Fornecedor &&
                f.CpfCnpj == cpfCnpj && (ignoreId == null || f.Id != ignoreId.Value));
        }

        public async Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefonePrincipal)) return false;

            return await _unitOfWork.FornecedorRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Fornecedor &&
                c.TelefonePrincipal == telefonePrincipal && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefoneWhatsApp)) return false;


            return await _unitOfWork.FornecedorRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Fornecedor &&
                c.TelefoneWhatsApp == telefoneWhatsApp && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> EmailExistsAsync(string email, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await _unitOfWork.FornecedorRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Fornecedor &&
                c.Email == email && (ignoreId == null || c.Id != ignoreId.Value));
        }
    }

}