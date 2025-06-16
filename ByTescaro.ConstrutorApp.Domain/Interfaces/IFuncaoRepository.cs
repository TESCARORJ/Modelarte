using ByTescaro.ConstrutorApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IFuncaoRepository : IRepository<Funcao>
    {
        Task<List<Funcao>> ObterTodasAsync();
        Task<Funcao?> ObterPorNomeAsync(string nome);
    }
}
