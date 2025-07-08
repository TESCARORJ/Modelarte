using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraServicoService : IObraServicoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ObraServicoService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string UsuarioLogado => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Desconhecido";


        public async Task<List<ObraServicoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _unitOfWork.ObraServicoRepository.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraServicoDto>>(itens);
        }

        public async Task CriarAsync(ObraServicoDto dto)
        {
            dto.DataHoraCadastro = DateTime.Now;
            dto.UsuarioCadastro = UsuarioLogado;
            var entity = _mapper.Map<ObraServico>(dto);
            _unitOfWork.ObraServicoRepository.Add(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task AtualizarAsync(ObraServicoDto dto)
        {
            var entity = await _unitOfWork.ObraServicoRepository.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            _unitOfWork.ObraServicoRepository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _unitOfWork.ObraServicoRepository.GetByIdAsync(id);
            if (entity != null)
                _unitOfWork.ObraServicoRepository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<ServicoDto>> ObterServicosDisponiveisAsync(long obraId)
        {
            var entidades = await _unitOfWork.ObraServicoRepository.GetServicosDisponiveisAsync(obraId);
            return _mapper.Map<List<ServicoDto>>(entidades);
        }

    }
}
