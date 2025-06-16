using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class OrcamentoObraService : IOrcamentoObraService
    {
        private readonly IOrcamentoObraRepository _repo;
        private readonly IMapper _mapper;

        public OrcamentoObraService(IOrcamentoObraRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<OrcamentoObraDto>> ObterPorObraIdAsync(long obraId)
        {
            var entidades = await _repo.GetByObraIdAsync(obraId);
            return _mapper.Map<List<OrcamentoObraDto>>(entidades);
        }


        public async Task<OrcamentoObraDto?> ObterPorIdComItensAsync(long id)
        {
            var entidade = await _repo.GetByIdWithItensAsync(id);
            return entidade is null ? null : _mapper.Map<OrcamentoObraDto>(entidade);
        }
    }
}
