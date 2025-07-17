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

                await _auditoriaService.RegistrarCriacaoAsync(configuracao, usuarioLogadoId);


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
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            // 1. Busque a entidade antiga SOMENTE PARA FINS DE AUDITORIA, SEM RASTREAMENTO.
            // Isso garante que essa instância de 'configuracaoParaAuditoria' não será modificada pelo AutoMapper.
            var configuracaoParaAuditoria = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdNoTrackingAsync(request.Id);

            if (configuracaoParaAuditoria == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para auditoria.");
            }

            // 2. Busque a entidade que REALMENTE SERÁ ATUALIZADA, COM RASTREAMENTO.
            // Essa é a instância que o EF Core vai monitorar para detectar as mudanças.
            var configuracaoParaAtualizar = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetByIdAsync(request.Id);

            if (configuracaoParaAtualizar == null)
            {
                throw new ApplicationException("Configuração de lembrete diário não encontrada para atualização.");
            }

            // 3. Mapeie as propriedades do 'request' para a entidade 'configuracaoParaAtualizar'.
            // O AutoMapper irá aplicar as mudanças diretamente em 'configuracaoParaAtualizar'.
            _mapper.Map(request, configuracaoParaAtualizar);

            // O método .Update() no repositório muitas vezes não é estritamente necessário se
            // a entidade já está rastreada e suas propriedades foram alteradas.
            // O EF Core já detecta as mudanças automaticamente.
            // _unitOfWork.ConfiguracaoLembreteDiarioRepository.Update(configuracaoParaAtualizar);

            // 4. Registre a auditoria, passando a cópia original e a entidade atualizada.
            // 'configuracaoParaAuditoria' tem os dados antes da mudança.
            // 'configuracaoParaAtualizar' tem os dados depois da mudança.
            await _auditoriaService.RegistrarAtualizacaoAsync(configuracaoParaAuditoria, configuracaoParaAtualizar, usuarioLogadoId);

            // 5. Salve as alterações no banco de dados.
            await _unitOfWork.CommitAsync();
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
            await _auditoriaService.RegistrarExclusaoAsync(configuracao, usuarioLogadoId);

            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<ConfiguracaoLembreteDiarioDto>> GetActiveDailyRemindersAsync()
        {
            var activeConfiguracoes = await _unitOfWork.ConfiguracaoLembreteDiarioRepository.GetActiveDailyRemindersAsync();
            return _mapper.Map<IEnumerable<ConfiguracaoLembreteDiarioDto>>(activeConfiguracoes);
        }
    }
}