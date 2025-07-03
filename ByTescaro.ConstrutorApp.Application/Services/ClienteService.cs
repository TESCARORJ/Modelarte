// Arquivo: Application/Services/ClienteService.cs

using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ClienteService : IClienteService
    {
        // Agora dependemos da Unit of Work, que nos dá acesso a todos os repositórios.
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteService(
            IUnitOfWork unitOfWork, // Injeta a IUnitOfWork em vez dos repositórios individuais
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogAuditoriaRepository logRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";

        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
        {
            var clientes = await _unitOfWork.ClienteRepository.GetAllAsync(); // Acessa o repositório via UnitOfWork
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto?> ObterPorIdAsync(long id)
        {
            var cliente = await _unitOfWork.ClienteRepository.GetByIdAsync(id);
            return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
        }

        public async Task CriarAsync(ClienteDto dto)
        {
            var entity = _mapper.Map<Cliente>(dto);
            entity.DataHoraCadastro = DateTime.UtcNow; // Use UtcNow para consistência
            entity.UsuarioCadastro = UsuarioLogado;

            // 1. Adiciona a entidade Cliente ao contexto (ainda não salva no banco)
            _unitOfWork.ClienteRepository.Add(entity);

            // 2. Adiciona o log de auditoria ao contexto (ainda não salva no banco)

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Cliente '{entity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(entity)
            });

            // 3. Salva TODAS as alterações (Cliente e Log) em uma única transação
            await _unitOfWork.CommitAsync();
        }

        // Arquivo: Application/Services/ClienteService.cs

        public async Task AtualizarAsync(ClienteDto dto)
        {
            // 1. Busque a entidade que será rastreada.
            //    Use GetByIdAsync ou um método que NÃO use AsNoTracking().
            var entidadeParaAtualizar = await _unitOfWork.ClienteRepository.GetByIdWithEnderecoAsync(dto.Id);

            if (entidadeParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {dto.Id} não encontrado.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar.
            var dadosAnteriores = JsonSerializer.Serialize(entidadeParaAtualizar);

            // 2. Use o AutoMapper para mapear as alterações do DTO PARA a entidade já existente.
            //    Isso atualiza as propriedades de 'entidadeParaAtualizar' em memória.
            _mapper.Map(dto, entidadeParaAtualizar);

            // 3. O EF Core já está rastreando 'entidadeParaAtualizar' e detectou as mudanças.
            //    A chamada a Update() aqui é opcional, mas boa prática para garantir o estado.
            _unitOfWork.ClienteRepository.Update(entidadeParaAtualizar);

            // 4. Adiciona o log de auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Atualizado",
                Descricao = $"Cliente '{entidadeParaAtualizar.Nome}' atualizado",
                DadosAnteriores = dadosAnteriores,
                DadosAtuais = JsonSerializer.Serialize(entidadeParaAtualizar) // Serializa a entidade já atualizada
            });

            // 5. Salva a única entidade modificada e o log em uma transação.
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ClienteRepository.GetByIdAsync(id);
            if (entity == null) return;

            // 1. Marca a entidade para remoção
            _unitOfWork.ClienteRepository.Remove(entity);

            // 2. Adiciona o log de auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Excluído",
                Descricao = $"Cliente '{entity.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(entity)
            });

            // 3. Salva TODAS as alterações em uma única transação
            await _unitOfWork.CommitAsync();
        }
    }
}