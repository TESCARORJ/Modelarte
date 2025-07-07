using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Funcionario> _funcionarioRepository;
        private readonly IRepository<Endereco> _enderecoRepository;
        private readonly ILogAuditoriaRepository _logRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FuncionarioService(
            ILogAuditoriaRepository logRepo,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IRepository<Funcionario> funcionarioRepository,
            IRepository<Endereco> enderecoRepository) 
        {
            _logRepo = logRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _funcionarioRepository = funcionarioRepository;
            _enderecoRepository = enderecoRepository;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";

        public async Task<IEnumerable<FuncionarioDto>> ObterTodosAsync()
        {
            var funcionarios = await _unitOfWork.FuncionarioRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios);
        }

        public async Task<FuncionarioDto?> ObterPorIdAsync(long id)
        {
            // Carrega com Endereco e Funcao para o DTO
            var funcionario = await _unitOfWork.FuncionarioRepository.FindOneWithIncludesAsync(f => f.Id == id, f => f.Endereco, f => f.Funcao);
            return funcionario == null ? null : _mapper.Map<FuncionarioDto>(funcionario);
        }

        public async Task CriarAsync(FuncionarioDto dto)
        {
            var funcionarioEntity = _mapper.Map<Funcionario>(dto);
            funcionarioEntity.DataHoraCadastro = DateTime.Now;
            funcionarioEntity.UsuarioCadastro = UsuarioLogado;
            funcionarioEntity.Ativo = true;
            funcionarioEntity.TipoEntidade = TipoEntidadePessoa.Funcionario;

            // Lógica para Endereço (Criação)
            if (!string.IsNullOrWhiteSpace(dto.CEP))
            {
                var enderecoEntity = _mapper.Map<Endereco>(dto);
                _enderecoRepository.Add(enderecoEntity);
                funcionarioEntity.Endereco = enderecoEntity;
            }

         
            _funcionarioRepository.Add(funcionarioEntity);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcionario),
                Acao = "Criado",
                Descricao = $"Funcionário '{funcionarioEntity.Nome}' criado",
                DadosAtuais = JsonSerializer.Serialize(dto)
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(FuncionarioDto dto)
        {
            // 1. Busque a entidade Funcionário COM O ENDEREÇO E FUNCAO INCLUÍDOS e rastreado
            var funcionarioToUpdate = await _funcionarioRepository.FindOneWithIncludesAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioToUpdate == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para atualização.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar.
            // IMPORTANTE: Mapear para um NOVO DTO para que a serialização não contenha referências de rastreamento.
            var dadosAnterioresDto = _mapper.Map<FuncionarioDto>(funcionarioToUpdate); // Mapeia entidade->DTO para log
            var dadosAnteriores = JsonSerializer.Serialize(dadosAnterioresDto); // Serializa o DTO

            // 2. Mapeie as propriedades do DTO de entrada para a entidade Funcionário existente e rastreada.
            _mapper.Map(dto, funcionarioToUpdate);

            // Garante que campos de auditoria e discriminador não sejam sobrescritos pelo DTO de entrada.
            // Estes valores devem vir da entidade original ou serem definidos pelo serviço.
            // Se o DTO tem esses campos, eles já foram mapeados acima, então esta parte é para proteger.
            // funcionarioToUpdate.UsuarioCadastro = funcionarioToUpdate.UsuarioCadastro; // Já está correto
            // funcionarioToUpdate.DataHoraCadastro = funcionarioToUpdate.DataHoraCadastro; // Já está correto
            funcionarioToUpdate.TipoEntidade = TipoEntidadePessoa.Funcionario;

            // 3. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
            if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
            {
                if (funcionarioToUpdate.Endereco == null) // Se o funcionário NÃO tinha endereço ANTES
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _enderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                    funcionarioToUpdate.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
                }
                else // Se o funcionário JÁ tinha endereço
                {
                    _mapper.Map(dto, funcionarioToUpdate.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                }
            }
            else // Se o DTO NÃO tem CEP, e o funcionário TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
            {
                if (funcionarioToUpdate.Endereco != null)
                {
                    _enderecoRepository.Remove(funcionarioToUpdate.Endereco); // Marca o endereço para remoção
                    funcionarioToUpdate.Endereco = null; // Desvincula o endereço do funcionário
                    funcionarioToUpdate.EnderecoId = null; // Garante que a FK também seja nullificada
                }
            }

            if (funcionarioToUpdate.EnderecoId == null)
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _enderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                funcionarioToUpdate.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
            }

         

            _funcionarioRepository.Update(funcionarioToUpdate); // Marca o funcionario como modificado (opcional se já rastreado)

            // 5. Adiciona o log de auditoria
            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcionario),
                Acao = "Atualizado",
                Descricao = $"Funcionário '{funcionarioToUpdate.Nome}' atualizado",
                DadosAnteriores = dadosAnteriores,
                DadosAtuais = JsonSerializer.Serialize(dto) // Serializa o DTO atual para o log
            });

            // 6. Salva TODAS as alterações em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var funcionarioToRemove = await _funcionarioRepository.FindOneWithIncludesAsync(f => f.Id == id, f => f.Endereco);
            if (funcionarioToRemove == null) return;

            if (funcionarioToRemove.Endereco != null)
            {
                _enderecoRepository.Remove(funcionarioToRemove.Endereco);
            }
            _funcionarioRepository.Remove(funcionarioToRemove);

            await _logRepo.RegistrarAsync(new LogAuditoria
            {
                Usuario = UsuarioLogado,
                Entidade = nameof(Funcionario),
                Acao = "Excluído",
                Descricao = $"Funcionário '{funcionarioToRemove.Nome}' removido",
                DadosAnteriores = JsonSerializer.Serialize(_mapper.Map<FuncionarioDto>(funcionarioToRemove))
            });

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj)) return false;
            string cleanedCpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            return await _funcionarioRepository.ExistsAsync(f =>
                f.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                f.CpfCnpj == cleanedCpfCnpj && (ignoreId == null || f.Id != ignoreId.Value));
        }

        public async Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefonePrincipal)) return false;
            string cleanedTelefone = new string(telefonePrincipal.Where(char.IsDigit).ToArray());
            return await _funcionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.TelefonePrincipal == cleanedTelefone && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefoneWhatsApp)) return false;
            string cleanedWhatsApp = new string(telefoneWhatsApp.Where(char.IsDigit).ToArray());
            return await _funcionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.TelefoneWhatsApp == cleanedWhatsApp && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> EmailExistsAsync(string email, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await _funcionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.Email == email && (ignoreId == null || c.Id != ignoreId.Value));
        }
    }
}