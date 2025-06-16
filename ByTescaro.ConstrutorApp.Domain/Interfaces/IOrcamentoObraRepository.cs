using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IOrcamentoObraRepository : IRepository<OrcamentoObra>
    {
        Task<OrcamentoObra?> GetByIdWithItensAsync(long id);
        Task<List<OrcamentoObra>> GetByObraIdAsync(long obraId);
    }
}
