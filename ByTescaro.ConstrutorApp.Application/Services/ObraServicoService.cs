using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoService : IObraServicoService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioLogadoService _usuarioLogadoService;

        public ObraServicoService(
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

        public async Task<List<ObraServicoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraServicoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraServicoDto>>(itens);
        }

        public async Task CriarAsync(ObraServicoDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var usuarioLogadoNome = usuarioLogado == null ? string.Empty : usuarioLogado.Nome;

            dto.DataHoraCadastro = DateTime.Now;
            dto.UsuarioCadastroId = usuarioLogadoId;
            dto.UsuarioCadastroNome = usuarioLogadoNome;
            var entity = _mapper.Map<ObraServico>(dto);
            _unitOfWork.ObraServicoRepository.Add(entity);

            await _unitOfWork.CommitAsync();
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);
        }

        public async Task AtualizarAsync(ObraServicoDto dto)
        {
            var usuarioLogado = await _usuarioLogadoService.ObterUsuarioAtualAsync();
            var usuarioLogadoId = usuarioLogado?.Id ?? 0;


            var obraServicoAntigoParaAuditoria = await _unitOfWork.ObraServicoRepository.GetByIdNoTrackingAsync(dto.Id);

            if (obraServicoAntigoParaAuditoria == null)
            {
                throw new KeyNotFoundException($"Obra Serviço com ID {dto.Id} não encontrado para auditoria.");
            }

            var obraServicoParaAtualizar = await _unitOfWork.ObraServicoRepository.GetByIdAsync(dto.Id);

            if (obraServicoParaAtualizar == null)
            {

                throw new KeyNotFoundException($"Obra Serviço com ID {dto.Id} não encontrado para atualização.");
            }

            
            _mapper.Map(dto, obraServicoParaAtualizar);

            await _unitOfWork.CommitAsync();
           
            await _auditoriaService.RegistrarAtualizacaoAsync(obraServicoAntigoParaAuditoria, obraServicoParaAtualizar, usuarioLogadoId);

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = await _unitOfWork.ObraServicoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraServicoRepository.Remove(entity);

            await _unitOfWork.CommitAsync();

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

        }

        public async Task<List<ServicoDto>> ObterServicosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraServicoRepository.GetServicosDisponiveisAsync(obraId);
            return _mapper.Map<List<ServicoDto>>(entidades);
        }

    }
}
