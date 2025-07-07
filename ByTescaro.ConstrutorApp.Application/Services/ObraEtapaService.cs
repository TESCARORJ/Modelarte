//using AutoMapper;
//using ByTescaro.ConstrutorApp.Application.DTOs;
//using ByTescaro.ConstrutorApp.Application.Interfaces;
//using ByTescaro.ConstrutorApp.Domain.Entities;
//using ByTescaro.ConstrutorApp.Domain.Enums;
//using ByTescaro.ConstrutorApp.Domain.Interfaces;

//namespace ByTescaro.ConstrutorApp.Application.Services
//{
//    public class ObraEtapaService : IObraEtapaService
//    {
//        private readonly IObraEtapaRepository _repo;
//        private readonly IMapper _mapper;

//        public ObraEtapaService(IObraEtapaRepository repo, IMapper mapper)
//        {
//            _repo = repo;
//            _mapper = mapper;
//        }

//        public async Task<List<ObraEtapaDto>> ObterTodasAsync()
//        {
//            var list = await _repo.GetAllAsync();
//            return _mapper.Map<List<ObraEtapaDto>>(list);
//        }

//        public async Task<ObraEtapaDto?> ObterPorIdAsync(long id)
//        {
//            var entity = await _repo.GetByIdAsync(id);
//            return _mapper.Map<ObraEtapaDto>(entity);
//        }

//        public async Task<List<ObraEtapaDto>> ObterPorObraIdAsync(long obraId)
//        {
//            var etapas = await _repo.GetByObraIdAsync(obraId);
//            return _mapper.Map<List<ObraEtapaDto>>(etapas);
//        }

//        public async Task<ObraEtapaDto?> ObterComItensAsync(long etapaId)
//        {
//            var etapa = await _repo.GetWithItensAsync(etapaId);
//            return _mapper.Map<ObraEtapaDto>(etapa);
//        }

//        public async Task AtualizarStatusAsync(long etapaId, StatusEtapa novoStatus)
//        {
//            var etapa = await _repo.GetByIdAsync(etapaId);
//            if (etapa == null) return;

//            etapa.Status = novoStatus;
//            _repo.Update(etapa);
//        }

//        public async Task CriarAsync(ObraEtapaDto dto)
//        {
//            var entity = _mapper.Map<ObraEtapa>(dto);
//            _repo.Add(entity);
//        }

//        public async Task AtualizarAsync(ObraEtapaDto dto)
//        {
//            var entity = await _repo.GetByIdAsync(dto.Id);
//            if (entity == null) return;

//            _mapper.Map(dto, entity);
//            _repo.Update(entity);
//        }

//        public async Task RemoverAsync(long id)
//        {
//            var entity = await _repo.GetByIdAsync(id);
//            if (entity != null) _repo.Remove(entity);
//        }
//    }
//}
