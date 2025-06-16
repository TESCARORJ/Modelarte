using ByTescaro.ConstrutorApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IOrcamentoItemService
    {
        Task<List<OrcamentoItemDto>> ObterPorOrcamentoIdAsync(long orcamentoId);
        Task CriarAsync(OrcamentoItemDto dto);
        Task AtualizarAsync(OrcamentoItemDto dto);
        Task RemoverAsync(long id);
    }

}
