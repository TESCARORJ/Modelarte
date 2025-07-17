using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraService : IObraService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUsuarioLogadoService _usuarioLogadoService;
        private readonly IAuditoriaService _auditoriaService;

        public ObraService(IUnitOfWork unitOfWork, IMapper mapper, IUsuarioLogadoService usuarioLogadoService, IAuditoriaService auditoriaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _usuarioLogadoService = usuarioLogadoService;
            _auditoriaService = auditoriaService;
        }

        public async Task<List<ObraDto>> ObterPorProjetoAsync(long projetoId)
        {
            var obras = await _unitOfWork.ObraRepository.GetByProjetoIdAsync(projetoId);
            return _mapper.Map<List<ObraDto>>(obras);
        }

        public async Task<ObraDto?> ObterPorIdAsync(long id)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(id);
            return _mapper.Map<ObraDto>(obra);
        }

        public async Task<ObraDto> CriarAsync(ObraDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entity = _mapper.Map<Obra>(dto);
            entity.DataHoraCadastro = DateTime.Now;
            entity.UsuarioCadastroId = usuarioLogadoId;

            _unitOfWork.ObraRepository.Add(entity);
            await _auditoriaService.RegistrarCriacaoAsync(entity, usuarioLogadoId);


            await _unitOfWork.CommitAsync();
            
            return _mapper.Map<ObraDto>(entity);


        }

        public async Task AtualizarAsync(ObraDto dto)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;
            var entityAntigo = await _unitOfWork.ObraRepository.GetByIdAsync(dto.Id);
            if (entityAntigo == null) return;

            var entityNovo = _mapper.Map(dto, entityAntigo);
            await _unitOfWork.ObraRepository.UpdateAsync(entityAntigo);
            await _auditoriaService.RegistrarAtualizacaoAsync(entityAntigo, entityNovo, usuarioLogadoId);


            await _unitOfWork.CommitAsync();

        }

        public async Task RemoverAsync(long id)
        {
            var usuarioLogado = _usuarioLogadoService.ObterUsuarioAtualAsync().Result;
            var usuarioLogadoId = usuarioLogado == null ? 0 : usuarioLogado.Id;

            var entity = await _unitOfWork.ObraRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraRepository.Remove(entity);

            await _auditoriaService.RegistrarExclusaoAsync(entity, usuarioLogadoId);

            await _unitOfWork.CommitAsync();

        }

        public async Task<int> CalcularProgressoAsync(long obraId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(obraId);
            if (obra == null || obra.Etapas.Count == 0)
                return 0;

            int progressoTotal = obra.Etapas.Sum(e => e.Itens.Count == 0 ? 0 :
                (int)Math.Round((double)e.Itens.Count(i => i.Concluido) / e.Itens.Count * 100));

            return (int)Math.Round((double)progressoTotal / obra.Etapas.Count);
        }

        public async Task<List<ObraDto>> ObterTodosAsync()
        {
            var obras = await _unitOfWork.ObraRepository.GetAllAsync();
            return _mapper.Map<List<ObraDto>>(obras);
        }

        public async Task<List<ObraEtapaDto>> ObterEtapasDaObraAsync(long obraId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(obraId);
            return _mapper.Map<List<ObraEtapaDto>>(obra?.Etapas?.ToList() ?? new List<ObraEtapa>());
        }

        public async Task<List<ObraItemEtapaDto>> ObterItensDaEtapaAsync(long etapaId)
        {
            var obra = await _unitOfWork.ObraRepository.GetByIdWithRelacionamentosAsync(etapaId);
            var etapa = obra?.Etapas.FirstOrDefault(e => e.Id == etapaId);
            var itens = etapa?.Itens?.ToList() ?? new List<ObraItemEtapa>();
            return _mapper.Map<List<ObraItemEtapaDto>>(itens);
        }


        public async Task AtualizarConclusaoItemAsync(long itemId, bool concluido)
        {
            var obra = await _unitOfWork.ObraRepository.GetByItemEtapaIdAsync(itemId);
            var item = obra?.Etapas.SelectMany(e => e.Itens).FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Concluido = concluido;
                await _unitOfWork.ObraRepository.UpdateAsync(obra!);
            }
        }

    }

}
