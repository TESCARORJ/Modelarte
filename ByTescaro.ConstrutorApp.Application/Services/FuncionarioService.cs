using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Enums;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IMapper _mapper;

        public FuncionarioService(
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


        public async Task<IEnumerable<FuncionarioDto>> ObterTodosAsync()
        {
            var funcionarios = await _unitOfWork.FuncionarioRepository.FindAllWithIncludesAsync(x => x.TipoEntidade == TipoEntidadePessoa.Funcionario, x => x.Endereco, x => x.Funcao);
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
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var funcionarioEntity = _mapper.Map<Funcionario>(dto);
            funcionarioEntity.DataHoraCadastro = DateTime.Now;
            funcionarioEntity.UsuarioCadastroId = usuarioLogadoId;
            funcionarioEntity.Ativo = true;
            funcionarioEntity.TipoEntidade = TipoEntidadePessoa.Funcionario;

            // Lógica para Endereço (Criação)
            if (!string.IsNullOrWhiteSpace(dto.CEP))
            {
                var enderecoEntity = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(enderecoEntity);
                funcionarioEntity.Endereco = enderecoEntity;
            }


            _unitOfWork.FuncionarioRepository.Add(funcionarioEntity);

            await _auditoriaService.RegistrarCriacaoAsync(funcionarioEntity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(FuncionarioDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            // 1. Busque a entidade Funcionário COM O ENDEREÇO E FUNCAO INCLUÍDOS e rastreado
            var funcionarioAntigo = await _unitOfWork.FuncionarioRepository.FindOneWithIncludesAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioAntigo == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para atualização.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar.
            // IMPORTANTE: Mapear para um NOVO DTO para que a serialização não contenha referências de rastreamento.
            var dadosAnterioresDto = _mapper.Map<FuncionarioDto>(funcionarioAntigo); // Mapeia entidade->DTO para log
            var dadosAnteriores = JsonSerializer.Serialize(dadosAnterioresDto); // Serializa o DTO

            // 2. Mapeie as propriedades do DTO de entrada para a entidade Funcionário existente e rastreada.
            var funcionarioNovo = _mapper.Map(dto, funcionarioAntigo);

            // Garante que campos de auditoria e discriminador não sejam sobrescritos pelo DTO de entrada.
            // Estes valores devem vir da entidade original ou serem definidos pelo serviço.
            // Se o DTO tem esses campos, eles já foram mapeados acima, então esta parte é para proteger.
            // funcionarioToUpdate.UsuarioCadastro = funcionarioToUpdate.UsuarioCadastro; // Já está correto
            // funcionarioToUpdate.DataHoraCadastro = funcionarioToUpdate.DataHoraCadastro; // Já está correto
            funcionarioNovo.TipoEntidade = TipoEntidadePessoa.Funcionario;

            // 3. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
            if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
            {
                if (funcionarioNovo.Endereco == null) // Se o funcionário NÃO tinha endereço ANTES
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                    funcionarioNovo.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
                }
                else // Se o funcionário JÁ tinha endereço
                {
                    _mapper.Map(funcionarioNovo, funcionarioAntigo.Endereco); // Mapeia DTO para o endereço existente (rastreado)
                }
            }
            else // Se o DTO NÃO tem CEP, e o funcionário TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
            {
                if (funcionarioNovo.Endereco != null)
                {
                    _unitOfWork.EnderecoRepository.Remove(funcionarioNovo.Endereco); // Marca o endereço para remoção
                    funcionarioNovo.Endereco = null; // Desvincula o endereço do funcionário
                    funcionarioNovo.EnderecoId = null; // Garante que a FK também seja nullificada
                }
            }

            if (funcionarioNovo.EnderecoId == null)
            {
                var novoEndereco = _mapper.Map<Endereco>(dto);
                _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                funcionarioNovo.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
            }



            _unitOfWork.FuncionarioRepository.Update(funcionarioNovo); // Marca o funcionario como modificado (opcional se já rastreado)

            // 5. Adiciona o log de auditoria
            await _auditoriaService.RegistrarAtualizacaoAsync(funcionarioAntigo, funcionarioNovo, usuarioLogadoId);


            // 6. Salva TODAS as alterações em uma única transação
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var funcionarioToRemove = await _unitOfWork.FuncionarioRepository.FindOneWithIncludesAsync(f => f.Id == id, f => f.Endereco, f => f.Funcao);
            if (funcionarioToRemove == null) return;

            if (funcionarioToRemove.Endereco != null)
            {
                _unitOfWork.EnderecoRepository.Remove(funcionarioToRemove.Endereco);
            }
            _unitOfWork.FuncionarioRepository.Remove(funcionarioToRemove);

            await _auditoriaService.RegistrarExclusaoAsync(funcionarioToRemove, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj)) return false;
            string cleanedCpfCnpj = new string(cpfCnpj.Where(char.IsDigit).ToArray());
            return await _unitOfWork.FuncionarioRepository.ExistsAsync(f =>
                f.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                f.CpfCnpj == cleanedCpfCnpj && (ignoreId == null || f.Id != ignoreId.Value));
        }

        public async Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefonePrincipal)) return false;
            string cleanedTelefone = new string(telefonePrincipal.Where(char.IsDigit).ToArray());
            return await _unitOfWork.FuncionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.TelefonePrincipal == cleanedTelefone && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(telefoneWhatsApp)) return false;
            string cleanedWhatsApp = new string(telefoneWhatsApp.Where(char.IsDigit).ToArray());
            return await _unitOfWork.FuncionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.TelefoneWhatsApp == cleanedWhatsApp && (ignoreId == null || c.Id != ignoreId.Value));
        }

        public async Task<bool> EmailExistsAsync(string email, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await _unitOfWork.FuncionarioRepository.ExistsAsync(c =>
                c.TipoEntidade == TipoEntidadePessoa.Funcionario &&
                c.Email == email && (ignoreId == null || c.Id != ignoreId.Value));
        }
    }
}