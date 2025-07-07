
using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Cliente> _clienteRepository;
        private readonly IRepository<Endereco> _enderecoRepository;

        public ClienteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogAuditoriaRepository logRepo,
            IRepository<Cliente> clienteRepository,
            IRepository<Endereco> enderecoRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logRepo = logRepo;
            _clienteRepository = clienteRepository;
            _enderecoRepository = enderecoRepository;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";

        public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
        {
            var clientes = await _unitOfWork.ClienteRepository.GetAllAsync(); // Acessa o repositório via UnitOfWork
            return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
        }

        public async Task<ClienteDto?> ObterPorIdAsync(long id)
        {
            var cliente = await _unitOfWork.ClienteRepository.FindOneWithIncludesAsync(c => c.Id == id, c => c.Endereco);
            return cliente == null ? null : _mapper.Map<ClienteDto>(cliente);
        }

        public async Task CriarAsync(ClienteDto dto)
        {
            var clienteEntity = _mapper.Map<Cliente>(dto);
            clienteEntity.DataHoraCadastro = DateTime.Now; // Use UtcNow para consistência
            clienteEntity.UsuarioCadastro = UsuarioLogado;
            clienteEntity.Ativo = true; // Clientes novos são ativos por padrão
            clienteEntity.TipoEntidade = TipoEntidadePessoa.Cliente;

            // Lógica para Endereço (Criação)
            if (!string.IsNullOrWhiteSpace(dto.CEP))
            {
                var enderecoEntity = _mapper.Map<Endereco>(dto); // Mapeia DTO para nova entidade Endereco
                _enderecoRepository.Add(enderecoEntity); // Adiciona o novo endereço ao contexto
                // O EnderecoId será populado após o SaveChangesAsync principal
                clienteEntity.Endereco = enderecoEntity; // Associa a entidade Endereco criada ao Cliente
            }

            _clienteRepository.Add(clienteEntity); // Adiciona o cliente ao contexto

            // Adiciona o log de auditoria ao contexto
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Criado",
                Descricao = $"Cliente '{clienteEntity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(dto) // Serializa o DTO para o log
            });

            // Salva TODAS as alterações (Cliente, Endereco (se houver) e Log) em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ClienteDto dto)
        {
            // 1. Busque a entidade Cliente COM O ENDEREÇO INCLUÍDO e rastreado
            var clienteToUpdate = await _clienteRepository.FindOneWithIncludesAsync(c => c.Id == dto.Id, c => c.Endereco);

            if (clienteToUpdate == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {dto.Id} não encontrado.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar
            var dadosAnteriores = JsonSerializer.Serialize(_mapper.Map<ClienteDto>(clienteToUpdate));

            // 2. Mapeie as propriedades do DTO para a entidade Cliente existente (exceto Endereco)
            // O AutoMapper por padrão não sobrescreve relacionamentos complexos como Endereco
            _mapper.Map(dto, clienteToUpdate);

            // Garante que campos de auditoria e discriminador não sejam sobrescritos
            clienteToUpdate.TipoEntidade = TipoEntidadePessoa.Cliente;

            // 3. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
            if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
            {
                if (clienteToUpdate.Endereco == null) // Se o cliente NÃO tinha endereço ANTES
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _enderecoRepository.Add(novoEndereco); // Adiciona o novo endereço
                    clienteToUpdate.Endereco = novoEndereco; // Associa o novo endereço ao cliente
                }
                else // Se o cliente JÁ tinha endereço
                {
                    _mapper.Map(dto, clienteToUpdate.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                    // Não é preciso _enderecoRepository.Update(clienteToUpdate.Endereco);
                    // O EF detectará as mudanças automaticamente porque ele já está rastreado.
                }
            }
            else // Se o DTO NÃO tem CEP, e o cliente TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
            {
                if (clienteToUpdate.Endereco != null)
                {
                    _enderecoRepository.Remove(clienteToUpdate.Endereco); // Marca o endereço para remoção
                    clienteToUpdate.Endereco = null; // Desvincula o endereço do cliente
                    clienteToUpdate.EnderecoId = null; // Garante que a FK também seja nullificada
                }
            }

            if (clienteToUpdate.EnderecoId == null)
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _enderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                clienteToUpdate.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
            }

            _clienteRepository.Update(clienteToUpdate); // Marca o cliente como modificado (ou as mudanças já foram detectadas)

            // 4. Adiciona o log de auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Atualizado",
                Descricao = $"Cliente '{clienteToUpdate.Nome}' atualizado",
                DadosAnteriores = dadosAnteriores,
                DadosAtuais = JsonSerializer.Serialize(dto) // Serializa o DTO atual para o log
            });

            // 5. Salva TODAS as alterações (Cliente, Endereço e Log) em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var clienteToRemove = await _clienteRepository.FindOneWithIncludesAsync(c => c.Id == id, c => c.Endereco);
            if (clienteToRemove == null) return;

            // Remove o endereço associado primeiro, se existir
            if (clienteToRemove.Endereco != null)
            {
                _enderecoRepository.Remove(clienteToRemove.Endereco);
            }
            _clienteRepository.Remove(clienteToRemove);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Cliente),
                Acao = "Excluído",
                Descricao = $"Cliente '{clienteToRemove.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(_mapper.Map<ClienteDto>(clienteToRemove))
            });

            await _unitOfWork.CommitAsync();
        }


        public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
            {
                return false;
            }

            return await _unitOfWork.ClienteRepository.ExistsAsync(f => f.TipoEntidade == TipoEntidadePessoa.Cliente &&
                f.CpfCnpj == cpfCnpj && (ignoreId == null || f.Id != ignoreId.Value));
        }

        public async Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefonePrincipal)) return false;
            
            return await _unitOfWork.ClienteRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Cliente &&
                c.TelefonePrincipal == telefonePrincipal && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefoneWhatsApp)) return false;


            return await _unitOfWork.ClienteRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Cliente &&
                c.TelefoneWhatsApp == telefoneWhatsApp && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> EmailExistsAsync(string email, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await _unitOfWork.ClienteRepository.ExistsAsync(c => c.TipoEntidade == TipoEntidadePessoa.Cliente &&
                c.Email == email && (ignoreId == null || c.Id != ignoreId.Value));
        }
    }
}