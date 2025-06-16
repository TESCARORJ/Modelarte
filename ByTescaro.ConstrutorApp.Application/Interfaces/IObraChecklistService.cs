using ByTescaro.ConstrutorApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IObraChecklistService
    {
        Task SalvarChecklistAsync(long obraId, List<ObraEtapaDto> etapas);
        Task<List<ObraEtapaDto>> ObterChecklistAsync(long obraId);
    }

}
