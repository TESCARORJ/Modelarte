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

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Essa instância 'fornecedorAntigoParaAuditoria' NÃO será modificada pelo AutoMapper,
            // preservando o estado original para o log.
            var fornecedorAntigoParaAuditoria = await _unitOfWork.FornecedorRepository.GetByIdNoTrackingAsync(dto.Id);

            if (fornecedorAntigoParaAuditoria == null)
            {
                // Se não encontrou, não há o que atualizar ou auditar para um ID existente.
                throw new KeyNotFoundException($"Fornecedor com ID {dto.Id} não encontrado para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa instância 'fornecedorParaAtualizar' é a que o EF Core está monitorando
            // e que terá suas propriedades alteradas.
            var fornecedorParaAtualizar = await _unitOfWork.FornecedorRepository.GetByIdAsync(dto.Id);

            if (fornecedorParaAtualizar == null)
            {
                // Isso deve ser raro se 'fornecedorAntigoParaAuditoria' foi encontrado,
                // mas é uma boa verificação de segurança.
                throw new KeyNotFoundException($"Fornecedor com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO para a entidade 'fornecedorParaAtualizar' (a rastreada).
            // O AutoMapper irá aplicar as mudanças DIRETAMENTE nesta instância.
            _mapper.Map(dto, fornecedorParaAtualizar);

            // Garanta que campos de auditoria de cadastro não sejam sobrescritos pelo DTO.
            // Você estava usando 'usuarioLogadoId' para 'UsuarioCadastroId' na sua versão,
            // mas para uma atualização, geralmente queremos manter o ID do usuário que CADASTROU.
            // Se 'UsuarioCadastroId' e 'DataHoraCadastro' não devem mudar na atualização,
            // reatribua os valores originais:
            fornecedorParaAtualizar.UsuarioCadastroId = fornecedorAntigoParaAuditoria.UsuarioCadastroId;
            fornecedorParaAtualizar.DataHoraCadastro = fornecedorAntigoParaAuditoria.DataHoraCadastro;

            // Garante que o tipo de entidade seja mantido, se for um campo discriminador.
            fornecedorParaAtualizar.TipoEntidade = TipoEntidadePessoa.Fornecedor;

            // A chamada a .Update() no repositório é geralmente redundante se a entidade já está
            // rastreada e suas propriedades foram alteradas diretamente. O EF Core detecta isso.
            // _unitOfWork.FornecedorRepository.Update(fornecedorParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'fornecedorAntigoParaAuditoria' tem os dados ANTES da mudança.
            // 'fornecedorParaAtualizar' tem os dados DEPOIS da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(fornecedorAntigoParaAuditoria, fornecedorParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
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