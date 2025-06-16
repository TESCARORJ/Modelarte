using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IUsuarioLogadoService
    {
        Task<Usuario?> ObterUsuarioAtualAsync();
    }
}
