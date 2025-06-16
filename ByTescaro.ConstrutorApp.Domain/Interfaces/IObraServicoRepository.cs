using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraServicoRepository : IRepository<ObraServico>
    {
        Task<List<ObraServico>> GetByListaIdAsync(long listaId);
        Task<List<Servico>> GetServicosDisponiveisAsync(long obraId);
    }
}
