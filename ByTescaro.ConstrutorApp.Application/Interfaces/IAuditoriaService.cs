namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IAuditoriaService
    {
        Task RegistrarCriacaoAsync<T>(T entidadeNova, long usuarioId) where T : class;
        Task RegistrarAtualizacaoAsync<T>(
        T entidadeAntiga,
        T entidadeNova,
        long usuarioId,
        Dictionary<string, Dictionary<long, string>>? colecoesNomes = null) where T : class;

        Task RegistrarExclusaoAsync<T>(T entidadeAntiga, long usuarioId) where T : class;
    }


}
