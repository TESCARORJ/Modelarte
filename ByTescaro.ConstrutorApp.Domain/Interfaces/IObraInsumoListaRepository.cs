using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraInsumoListaRepository : IRepository<ObraInsumoLista>
    {
        Task<List<ObraInsumoLista>> GetByObraIdAsync(long obraId);
        Task<ObraInsumoLista?> GetByIdWithItensAsync(long id);
        Task<ObraInsumoLista?> GetByIdWithItensNoTrackingAsync(long id);
    }
}
