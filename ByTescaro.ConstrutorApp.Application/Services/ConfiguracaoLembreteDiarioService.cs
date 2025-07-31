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
    public class ConfiguracaoLembreteDiarioService : IConfiguracaoLembreteDiarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IAuditoriaService _auditoriaService;


        public ConfiguracaoLembreteDiarioService(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioLogadoService usuarioLogadoService, IAuditoriaService auditoriaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
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
                var usuarioLogado =  _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
                var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

                var configuracao = _mapper.Map<ConfiguracaoLembreteDiario>(request);
                configuracao.DataHoraCadastro = DateTime.Now;
                configuracao.UsuarioCadastroId = usuarioLogado.Id; // Obtém o ID do usuário logado

                _unitOfWork.ConfiguracaoLembreteDiarioRepository.Add(configuracao);

                await _unitOfWork.CommitAsync();
                await _auditoriaService.RegistrarCriacaoAsync(configuracao, usuarioLogadoId);

                return _mapper.Map<ConfiguracaoLembreteDiarioDto>(configuracao);
            }
            catch (Exception ex)
            {

                throw ex;
            }
      
        }

        public async Task UpdateAsync(AtualizarConfiguracaoLembreteDiarioRequest request)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            
            var configuracaoParaAuditoria = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdNoTrackingAsync(request.Id);

            if (configuracaoParaAuditoria == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para auditoria.");
            }

          
            var configuracaoParaAtualizar = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(request.Id);

            if (configuracaoParaAtualizar == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para atualização.");
            }

           
            _mapper.Map(request, configuracaoParaAtualizar);

           
            _unitOfWork.ConfiguracaoLembreteDiarioRepository.Update(configuracaoParaAtualizar);
            
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(configuracaoParaAuditoria, configuracaoParaAtualizar, usuarioLogadoId);
        }

        public async Task DeleteAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var configuracao = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(id);
            if (configuracao == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para exclusão.");
            }

            _unitOfWork.ConfiguracaoLembreteDiarioRepository.Remove(configuracao);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(configuracao, usuarioLogadoId);
        }

        public async Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetActiveDailyRemindersAsync()
        {
            var activeConfiguracoes = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetActiveDailyRemindersAsync();
            return _mapper.Map<IEnumerable<ConfiguracaoLembreteDiarioDto>>(activeConfiguracoes);
        }
    }
}