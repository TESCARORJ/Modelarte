
using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ClienteService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUsuarioLogadoService usuarioLogadoService,
            IAuditoriaService auditoriaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
        }


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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var clienteEntity = _mapper.Map<Cliente>(dto);
            clienteEntity.DataHoraCadastro = DateTime.Now; // Use UtcNow para consistência
            clienteEntity.UsuarioCadastroId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            clienteEntity.Ativo = true; // Clientes novos são ativos por padrão
            clienteEntity.TipoEntidade = TipoEntidadePessoa.Cliente;

            // Lógica para Endereço (Criação)
            if (!string.IsNullOrWhiteSpace(dto.CEP))
            {
                var enderecoEntity = _mapper.Map<Endereco>(dto); // Mapeia DTO para nova entidade Endereco
                _unitOfWork.EnderecoRepository.Add(enderecoEntity); // Adiciona o novo endereço ao contexto
                // O EnderecoId será populado após o SaveChangesAsync principal
                clienteEntity.Endereco = enderecoEntity; // Associa a entidade Endereco criada ao Cliente
            }

            _unitOfWork.ClienteRepository.Add(clienteEntity); // Adiciona o cliente ao contexto

            await _auditoriaService.RegistrarCriacaoAsync(clienteEntity, usuarioLogadoId);


            // Salva TODAS as alterações (Cliente, Endereco (se houver) e Log) em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ClienteDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;


            // 1. Busque a entidade Cliente COM O ENDEREÇO INCLUÍDO e rastreado
            var entityAntigo = await _unitOfWork.ClienteRepository.FindOneWithIncludesAsync(c => c.Id == dto.Id, c => c.Endereco);

            if (entityAntigo == null)
            {
                throw new KeyNotFoundException($"Cliente com ID {dto.Id} não encontrado.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar
            var dadosAnteriores = JsonSerializer.Serialize(_mapper.Map<ClienteDto>(entityAntigo));

            // 2. Mapeie as propriedades do DTO para a entidade Cliente existente (exceto Endereco)

            // Garante que campos de auditoria e discriminador não sejam sobrescritos
            entityAntigo.TipoEntidade = TipoEntidadePessoa.Cliente;

            // O AutoMapper por padrão não sobrescreve relacionamentos complexos como Endereco
            var entityNovo = _mapper.Map(dto, entityAntigo);

            // 3. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
            if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
            {
                if (entityNovo.Endereco == null) // Se o cliente NÃO tinha endereço ANTES
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço
                    entityNovo.Endereco = novoEndereco; // Associa o novo endereço ao cliente
                }
                else // Se o cliente JÁ tinha endereço
                {
                    _mapper.Map(dto, entityAntigo.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                    // Não é preciso _enderecoRepository.Update(clienteToUpdate.Endereco);
                    // O EF detectará as mudanças automaticamente porque ele já está rastreado.
                }
            }
            else // Se o DTO NÃO tem CEP, e o cliente TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
            {
                if (entityNovo.Endereco != null)
                {
                    _unitOfWork.EnderecoRepository.Remove(entityNovo.Endereco); // Marca o endereço para remoção
                    entityNovo.Endereco = null; // Desvincula o endereço do cliente
                    entityNovo.EnderecoId = null; // Garante que a FK também seja nullificada
                }
            }

            if (entityNovo.EnderecoId == null)
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                entityNovo.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
            }

            _unitOfWork.ClienteRepository.Update(entityNovo); // Marca o cliente como modificado (ou as mudanças já foram detectadas)

            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);


            // 5. Salva TODAS as alterações (Cliente, Endereço e Log) em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var clienteToRemove = await _unitOfWork.ClienteRepository.FindOneWithIncludesAsync(c => c.Id == id, c => c.Endereco);
            if (clienteToRemove == null) return;

            // Remove o endereço associado primeiro, se existir
            if (clienteToRemove.Endereco != null)
            {
                _unitOfWork.EnderecoRepository.Remove(clienteToRemove.Endereco);
            }
            _unitOfWork.ClienteRepository.Remove(clienteToRemove);


            await _auditoriaService.RegistrarExclusaoAsync(clienteToRemove, usuarioLogadoId);

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