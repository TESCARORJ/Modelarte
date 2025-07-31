using AutoMapper;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class PersonalizacaoService : IPersonalizacaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PersonalizacaoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PersonalizacaoDto> ObterAsync()
        {
            var personalizacao = await _unitOfWork.PersonalizacaoRepository.ObterUnicaAsync();
            return _mapper.Map<PersonalizacaoDto>(personalizacao);
        }

        public async Task AtualizarAsync(PersonalizacaoDto dto)
        {
            var personalizacao = await _unitOfWork.PersonalizacaoRepository.ObterUnicaAsync();
            _mapper.Map(dto, personalizacao);
            _unitOfWork.PersonalizacaoRepository.Update(personalizacao);
            await _unitOfWork.CommitAsync();
        }
    }
}
