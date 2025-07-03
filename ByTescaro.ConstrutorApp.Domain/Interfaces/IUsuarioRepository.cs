using ByTescaro.ConstrutorApp.Domain.Entities;
using ByTescaro.ConstrutorApp.Domain.Entities.Admin;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        void UpdateDetached(Usuario entit);
        Task<Usuario?> ObterPorEmailComPerfilAsync(string email);


    }
}
