using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IFornecedorServicoRepository : IRepository<FornecedorServico>
    {
        Task<List<FornecedorServico>> ObterPorFornecedorIdAsync(long fornecedorId);
        Task<List<FornecedorServico>> ObterPorServicoIdAsync(long servicoId);
    }

}
