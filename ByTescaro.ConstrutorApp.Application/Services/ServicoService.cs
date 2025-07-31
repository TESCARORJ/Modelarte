using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{


    public class ServicoService : IServicoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;


        public ServicoService(
            IAuditoriaService auditoriaService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUsuarioLogadoService usuarioLogadoService)
        {
            _auditoriaService = auditoriaService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _usuarioLogadoService = usuarioLogadoService;
        }



        public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
        {
            var insumos = await _unitOfWork.ServicoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServicoDto>>(insumos);
        }

        public async Task<ServicoDto?> ObterPorIdAsync(long id)
        {
            var insumo = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            return insumo == null ? null : _mapper.Map<ServicoDto>(insumo);
        }

        public async Task CriarAsync(ServicoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = _mapper.Map<Servico>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ServicoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ServicoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;

            var servicoParaAtualizar = await _unitOfWork.ServicoRepository.GetByIdAsync(dto.Id);

            if (servicoParaAtualizar == null)
            {
                throw new KeyNotFoundException($"Serviço com ID {dto.Id} não encontrado para atualização.");
            }


            var servicoAntigoParaAuditoria = _mapper.Map<Servico>(servicoParaAtualizar);

            _mapper.Map(dto, servicoParaAtualizar);


            _unitOfWork.ServicoRepository.Update(servicoParaAtualizar);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarAtualizacaoAsync(servicoAntigoParaAuditoria, servicoParaAtualizar, usuarioLogadoId);
        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ServicoRepository.GetByIdAsync(id);
            if (entity == null) return;

            _unitOfWork.ServicoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);
        }

        public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(nome)) return false;
            return await _unitOfWork.ServicoRepository.ExistsAsync(e =>
                e.Nome == nome && (ignoreId == null || e.Id != ignoreId.Value));
        }
    }

}