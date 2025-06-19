using ByTescaro.ConstrutorApp.Domain.Entities;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraInsumoRepository : IRepository<ObraInsumo>
    {
        Task<List<ObraInsumo>> GetByListaIdAsync(long listaId);
        Task<List<Insumo>> GetInsumosDisponiveisAsync(long obraId);
        Task<List<Insumo>> GetInsumosPorPadraoObraAsync(long obraId);
    }
}
