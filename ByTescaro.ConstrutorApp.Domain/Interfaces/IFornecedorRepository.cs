using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IFornecedorRepository : IRepository<Fornecedor>
{
    Task<Fornecedor?> ObterPorCpfCnpjAsync(string cpfCnpj);
}
