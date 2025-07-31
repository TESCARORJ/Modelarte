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
        public async Task<IEnumerable<FuncionarioDto>> ObterTodosAtivosAsync()
        {
            var funcionarios = await _unitOfWork.FuncionarioRepository.FindAllWithIncludesAsync(x => x.TipoEntidade == TipoEntidadePessoa.Funcionario && x.Ativo == true, x => x.Endereco, x => x.Funcao);
            return _mapper.Map<IEnumerable<FuncionarioDto>>(funcionarios);
        }

        public async Task<FuncionarioDto?> ObterPorIdAsync(long id)
        {
            // Carrega com Endereco, Funcao E UsuarioCadastro (e UsuarioAtualizacao se existir)
            // Uma única consulta ao banco de dados para obter todos os dados necessários.
            var funcionario = await _unitOfWork.FuncionarioRepository.FindOneWithIncludesAsync(
                f => f.Id == id,
                f => f.Endereco,
                f => f.Funcao,
                f => f.UsuarioCadastro // <<--- Inclua aqui!
                                       // f => f.UsuarioAtualizacao // <<--- Inclua aqui se necessário!
            );

            if (funcionario == null)
            {
                return null; // Retorna null se não encontrar o funcionário
            }

            // O mapeamento agora preencherá UsuarioCadastroNome (e UsuarioAtualizacaoNome) automaticamente
            // porque a propriedade de navegação UsuarioCadastro (e UsuarioAtualizacao) foi incluída na busca.
            var dto = _mapper.Map<FuncionarioDto>(funcionario);

            return dto;
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

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(funcionarioEntity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(FuncionarioDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            
            var funcionarioAntigoParaAuditoria = await _unitOfWork.FuncionarioRepository
                .FindOneWithIncludesNoTrackingAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para auditoria.");
            }

         
            var dadosAnteriores = JsonSerializer.Serialize(_mapper.Map<FuncionarioDto>(funcionarioAntigoParaAuditoria));

           
            var funcionarioParaAtualizar = await _unitOfWork.FuncionarioRepository
                .FindOneWithIncludesAsync(f => f.Id == dto.Id, f => f.Endereco, f => f.Funcao);

            if (funcionarioParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Funcionário com ID {dto.Id} não encontrado para atualização.");
            }

          
            _mapper.Map(dto, funcionarioParaAtualizar);

           
            funcionarioParaAtualizar.UsuarioCadastroId = funcionarioAntigoParaAuditoria.UsuarioCadastroId;
            funcionarioParaAtualizar.DataHoraCadastro = funcionarioAntigoParaAuditoria.DataHoraCadastro;
            funcionarioParaAtualizar.TipoEntidade = TipoEntidadePessoa.Funcionario; 

            if (!string.IsNullOrWhiteSpace(dto.CEP)) 
            {
                if (funcionarioParaAtualizar.Endereco == null)
                {
                    var novoEndereco = _mapper.Map<Endereco>(dto);
                    _unitOfWork.EnderecoRepository.Add(novoEndereco); 
                    funcionarioParaAtualizar.Endereco = novoEndereco; 
                }
                else
                {
                    
                    _mapper.Map(dto, funcionarioParaAtualizar.Endereco);
                }
            }
            else 
            {
                if (funcionarioParaAtualizar.Endereco != null)
                {
                    _unitOfWork.EnderecoRepository.Remove(funcionarioParaAtualizar.Endereco); 
                    funcionarioParaAtualizar.Endereco = null; 
                    funcionarioParaAtualizar.EnderecoId = null; 
                }
            }

           
            await _auditoriaService.RegistrarAtualizacaoAsync(funcionarioAntigoParaAuditoria, funcionarioParaAtualizar, usuarioLogadoId);

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

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(funcionarioToRemove, usuarioLogadoId);


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