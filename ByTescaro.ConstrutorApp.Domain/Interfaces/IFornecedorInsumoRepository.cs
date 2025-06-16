using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IFornecedorInsumoRepository : IRepository<FornecedorInsumo>
    {
        Task<List<FornecedorInsumo>> ObterPorFornecedorIdAsync(long fornecedorId);
        Task<List<FornecedorInsumo>> ObterPorInsumoIdAsync(long insumoId);
    }

}
