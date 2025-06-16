using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IAuditoriaService
    {
        Task RegistrarCriacaoAsync<T>(T entidadeNova, string usuario) where T : class;
        Task RegistrarAtualizacaoAsync<T>(
        T entidadeAntiga,
        T entidadeNova,
        string usuario,
        Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class;

        Task RegistrarExclusaoAsync<T>(T entidadeAntiga, string usuario) where T : class;
    }


}
