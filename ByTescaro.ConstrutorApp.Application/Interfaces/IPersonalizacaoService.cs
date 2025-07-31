using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IPersonalizacaoService
    {
        Task<PersonalizacaoDto> ObterAsync();
        Task AtualizarAsync(PersonalizacaoDto dto);
    }
}
