using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task<Cliente?> GetByIdWithEnderecoAsync(long id);

}
