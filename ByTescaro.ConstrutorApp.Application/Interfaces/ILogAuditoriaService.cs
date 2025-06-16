using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface ILogAuditoriaService
    {
        Task<List<LogAuditoria>> ObterTodosAsync();
    }

}
