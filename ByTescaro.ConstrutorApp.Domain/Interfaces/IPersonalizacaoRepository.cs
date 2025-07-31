using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IPersonalizacaoRepository : IRepository<Personalizacao>
    {
        Task<Personalizacao> ObterUnicaAsync();
    }
}
