using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ObraItemEtapaService : IObraItemEtapaService
    {
        private readonly IObraItemEtapaRepository _repo;
        private readonly IMapper _mapper;

        public ObraItemEtapaService(IObraItemEtapaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ObraItemEtapaDto>> ObterTodasAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<List<ObraItemEtapaDto>>(list);
        }

        public async Task<List<ObraItemEtapaDto>> ObterPorEtapaIdAsync(long etapaId)
        {
            var itens = await _repo.GetByEtapaIdAsync(etapaId);
            return _mapper.Map<List<ObraItemEtapaDto>>(itens);
        }

        public async Task AtualizarConclusaoAsync(long itemId, bool concluido)
        {
            var item = await _repo.GetByIdAsync(itemId);
            if (item == null) return;

            item.Concluido = concluido;
            _repo.Update(item);
        }

        public async Task CriarAsync(ObraItemEtapaDto dto)
        {
            var entity = _mapper.Map<ObraItemEtapa>(dto);
            _repo.Add(entity);
        }

        public async Task AtualizarAsync(ObraItemEtapaDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return;

            _mapper.Map(dto, entity);
            _repo.Update(entity);
        }

        public async Task RemoverAsync(long id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
                _repo.Remove(entity);
        }
    }
}
