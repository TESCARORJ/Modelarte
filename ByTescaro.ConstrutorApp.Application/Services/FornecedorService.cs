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
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;


        public FornecedorService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IAuditoriaService auditoriaService,
            IUsuarioLogadoService usuarioLogadoService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _auditoriaService = auditoriaService;
            _usuarioLogadoService = usuarioLogadoService;
        }


        public async Task<IEnumerable<FornecedorDto>> ObterTodosAsync()
        {
            var fornecedores = await _unitOfWork.FornecedorRepository.FindAllWithIncludesAsync(x => x.TipoEntidade == TipoEntidadePessoa.Fornecedor, x => x.Endereco);
            return _mapper.Map<IEnumerable<FornecedorDto>>(fornecedores);
        }

        public async Task<FornecedorDto?> ObterPorIdAsync(long id)
        {
            var fornecedor = await _unitOfWork.FornecedorRepository.GetByIdAsync(id);
            return fornecedor == null ? null : _mapper.Map<FornecedorDto>(fornecedor);
        }

        public async Task CriarAsync(FornecedorDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Fornecedor>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            entity.TipoEntidade = TipoEntidadePessoa.Fornecedor;
            _unitOfWork.FornecedorRepository.Add(entity);

            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();

        }

        public async Task AtualizarAsync(FornecedorDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var fornecedorParaAtualizar = await _unitOfWork.FornecedorRepository.GetByIdAsync(dto.Id);

            if (fornecedorParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Fornecedor com ID {dto.Id} não encontrado para atualização.");
            }

          
            var fornecedorAntigoParaAuditoria = _mapper.Map<Fornecedor>(fornecedorParaAtualizar);
            
            _mapper.Map(dto, fornecedorParaAtualizar);

            fornecedorParaAtualizar.TipoEntidade = TipoEntidadePessoa.Fornecedor;

            _unitOfWork.FornecedorRepository.Update(fornecedorParaAtualizar);
            await _auditoriaService.RegistrarAtualizacaoAsync(fornecedorAntigoParaAuditoria, fornecedorParaAtualizar, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.FornecedorRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.FornecedorRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);


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