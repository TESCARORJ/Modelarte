using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface IAgendaService
    {
        Task<EventoDto> CriarEventoAsync(CriarEventoRequest request, long criadorId);
        Task<EventoDto> AtualizarEventoAsync(AtualizarEventoRequest request, long usuarioId);
        Task ExcluirEventoAsync(long eventoId, long usuarioId);
        Task<IEnumerable<EventoDto>> ObterEventosDoUsuarioAsync(long usuarioId);
        Task<EventoDto> ObterEventoPorIdAsync(long eventoId, long usuarioId);
        Task ResponderConviteAsync(RespostaConviteRequest request, long usuarioId);
        Task EnviarLembretesProximosEventosAsync(); // Para serviço de background
    }
}
