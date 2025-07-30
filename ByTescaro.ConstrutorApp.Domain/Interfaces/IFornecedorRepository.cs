using ByTescaro.ConstrutorApp.Domain.Entities;
using System.Linq.Expressions;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces;

public interface IFornecedorRepository : IRepository<Fornecedor>
{
    Task<Fornecedor?> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task<List<Fornecedor>> GetAllIncludingAsync(params Expression<Func<Fornecedor, object>>[] includes);

}
