using ByTescaro.ConstrutorApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{

    public interface IOrcamentoService
    {
        Task<List<OrcamentoDto>> ObterTodosAsync();
        Task<OrcamentoDto> CriarAsync(OrcamentoDto dto);
        Task<OrcamentoDto?> ObterPorIdAsync(long id);
        Task<List<OrcamentoDto>> ObterPorObraAsync(long obraId);
        Task<OrcamentoDto?> ObterPorIdComItensAsync(long id);
        Task AtualizarAsync(OrcamentoDto dto);
        Task RemoverAsync(long id);
    }



}
