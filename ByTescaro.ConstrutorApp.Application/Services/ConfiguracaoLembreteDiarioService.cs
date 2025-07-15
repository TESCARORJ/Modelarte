using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ConfiguracaoLembreteDiarioService : IConfiguracaoLembreteDiarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ConfiguracaoLembreteDiarioService(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioLogadoService usuarioLogadoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioLogadoService = usuarioLogadoService;
        }

        public async Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetAllAsync()
        {
            var configuracoes = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ConfiguracaoLembreteDiarioDto>>(configuracoes);
        }

        public async Task<ConfiguracaoLembreteDiarioDto?> GetByIdAsync(long id)
        {
            var configuracao = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(id);
            return _mapper.Map<ConfiguracaoLembreteDiarioDto>(configuracao);
        }

        public async Task<ConfiguracaoLembreteDiarioDto> CreateAsync(CriarConfiguracaoLembreteDiarioRequest request)
        {
            try
            {
                var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
                var configuracao = _mapper.Map<ConfiguracaoLembreteDiario>(request);
                configuracao.DataHoraCadastro = DateTime.Now;
                configuracao.UsuarioCadastroId = usuarioLogado.Id; // Obtém o ID do usuário logado

                _unitOfWork.ConfiguracaoLembreteDiarioRepository.Add(configuracao);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<ConfiguracaoLembreteDiarioDto>(configuracao);
            }
            catch (Exception ex)
            {

                throw ex;
            }
      
        }

        public async Task UpdateAsync(AtualizarConfiguracaoLembreteDiarioRequest request)
        {
            var configuracaoExistente = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(request.Id);

            if (configuracaoExistente == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada.");
            }

            _mapper.Map(request, configuracaoExistente); // Mapeia as propriedades atualizáveis

            _unitOfWork.ConfiguracaoLembreteDiarioRepository.Update(configuracaoExistente); // Marca como modificado
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var configuracao = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(id);
            if (configuracao == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para exclusão.");
            }

            _unitOfWork.ConfiguracaoLembreteDiarioRepository.Remove(configuracao);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetActiveDailyRemindersAsync()
        {
            var activeConfiguracoes = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetActiveDailyRemindersAsync();
            return _mapper.Map<IEnumerable<ConfiguracaoLembreteDiarioDto>>(activeConfiguracoes);
        }
    }
}