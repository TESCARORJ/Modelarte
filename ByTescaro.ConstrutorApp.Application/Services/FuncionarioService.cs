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
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade Funcionário original COM INCLUDES e SEM RASTREAMENTO.
            // Esta é a CÓPIA para auditoria, que não deve ser modificada pelo AutoMapper.
            var funcionarioAntigoParaAuditoria = await _unitOfWork.FuncionarioRepository
                .FindOneWithIncludesNoTrackingAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para auditoria.");
            }

            // Armazena uma cópia do estado antigo para o log de auditoria ANTES de modificar.
            // Mapeia para um DTO (ou um objeto JSON) para garantir que a serialização não contenha referências de rastreamento do EF Core.
            var dadosAnteriores = JsonSerializer.Serialize(_mapper.Map<FuncionarioDto>(funcionarioAntigoParaAuditoria));

            // 2. Busque a entidade Funcionário que será ATUALIZADA COM INCLUDES e COM RASTREAMENTO.
            // Esta é a entidade que o AutoMapper e o EF Core irão realmente modificar e rastrear.
            var funcionarioParaAtualizar = await _unitOfWork.FuncionarioRepository
                .FindOneWithIncludesAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para atualização.");
            }

            // 3. Mapeie as propriedades do DTO de entrada para a entidade Funcionário existente e rastreada.
            // O AutoMapper irá aplicar as mudanças diretamente em 'funcionarioParaAtualizar'.
            _mapper.Map(dto, funcionarioParaAtualizar);

            // Garante que campos de auditoria de criação não sejam sobrescritos pelo DTO de entrada.
            // Estes valores devem vir da entidade original (antes da atualização).
            funcionarioParaAtualizar.UsuarioCadastroId = funcionarioAntigoParaAuditoria.UsuarioCadastroId;
            funcionarioParaAtualizar.DataHoraCadastro = funcionarioAntigoParaAuditoria.DataHoraCadastro;
            funcionarioParaAtualizar.TipoEntidade = TipoEntidadePessoa.Funcionario; // Garante o discriminador, se aplicável.

            // 4. Lógica para ATUALIZAR/CRIAR/REMOVER a entidade Endereco
            if (!string.IsNullOrWhiteSpace(dto.CEP)) // Se o DTO tem dados de endereço
            {
                if (funcionarioParaAtualizar.Endereco == null) // Se o funcionário NÃO tinha endereço ANTES
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _unitOfWork.EnderecoRepository.Add(novoEndereco); // Adiciona o novo endereço ao contexto
                    funcionarioParaAtualizar.Endereco = novoEndereco; // Associa o novo endereço ao funcionário
                }
                else // Se o funcionário JÁ tinha endereço
                {
                    // Mapeia DTO para o endereço existente (rastreado).
                    // Use 'dto' como origem e 'funcionarioParaAtualizar.Endereco' como destino.
                    _mapper.Map(dto, funcionarioParaAtualizar.Endereco);
                }
            }
            else // Se o DTO NÃO tem CEP, e o funcionário TINHA endereço, REMOVER/DESVINCULAR o Endereço existente
            {
                if (funcionarioParaAtualizar.Endereco != null)
                {
                    _unitOfWork.EnderecoRepository.Remove(funcionarioParaAtualizar.Endereco); // Marca o endereço para remoção
                    funcionarioParaAtualizar.Endereco = null; // Desvincula o endereço do funcionário
                    funcionarioParaAtualizar.EnderecoId = null; // Garante que a FK também seja nullificada
                }
            }

            // A linha abaixo é redundante e pode causar um novo endereço se o EnderecoId for null por outro motivo.
            // A lógica acima já trata a criação ou atualização. Remova-a.
            // if (funcionarioNovo.EnderecoId == null)
            // {
            //     var novoEndereco = _mapper.Map<Endereco>(dto);
            //     _unitOfWork.EnderecoRepository.Add(novoEndereco);
            //     funcionarioNovo.Endereco = novoEndereco;
            // }

            // O método .Update() no repositório muitas vezes não é estritamente necessário se
            // a entidade (funcionarioParaAtualizar) já está rastreada e suas propriedades foram alteradas.
            // O EF Core já detecta as mudanças automaticamente.
            // _unitOfWork.FuncionarioRepository.Update(funcionarioParaAtualizar);

            // 5. Adiciona o log de auditoria.
            // Passe 'funcionarioAntigoParaAuditoria' (o estado antes) e 'funcionarioParaAtualizar' (o estado depois).
            await _auditoriaService.RegistrarAtualizacaoAsync(funcionarioAntigoParaAuditoria, funcionarioParaAtualizar, usuarioLogadoId);

            // 6. Salva TODAS as alterações em uma única transação.
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