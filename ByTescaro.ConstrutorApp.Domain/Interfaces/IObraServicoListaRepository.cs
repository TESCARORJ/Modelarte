using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IObraServicoListaRepository : IRepository<ObraServicoLista>
    {
        Task<List<ObraServicoLista>> GetByObraIdAsync(long obraId);
        Task<ObraServicoLista?> GetByIdWithItensAsync(long id);
        Task<ObraServicoLista?> GetByIdWithItensNoTrackingAsync(long id);
    }
}
