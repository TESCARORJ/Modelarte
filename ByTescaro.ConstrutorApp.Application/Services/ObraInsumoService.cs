using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraInsumoService : IObraInsumoService
    {
        private readonly IObraInsumoRepository _repo;
        private readonly IMapper _mapper;

        public ObraInsumoService(IObraInsumoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ObraInsumoDto>> ObterPorListaIdAsync(long listaId)
        {
            var itens = await _repo.GetByListaIdAsync(listaId);
            return _mapper.Map<List<ObraInsumoDto>>(itens);
        }

        public async Task CriarAsync(ObraInsumoDto dto)
        {
            var entity = _mapper.Map<ObraInsumo>(dto);
            await _repo.AddAsync(entity);
        }

        public async Task AtualizarAsync(ObraInsumoDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                await _repo.RemoveAsync(entity);
        }

        public async Task<List<InsumoDto>> ObterInsumosDisponiveisAsync(long obraId)
        {
            var entidades = await _repo.GetInsumosDisponiveisAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

        public async Task<List<InsumoDto>> ObterInsumosPorPadraoObraAsync(long obraId)
        {
            var entidades = await _repo.GetInsumosPorPadraoObraAsync(obraId);
            return _mapper.Map<List<InsumoDto>>(entidades);
        }

    }
}
