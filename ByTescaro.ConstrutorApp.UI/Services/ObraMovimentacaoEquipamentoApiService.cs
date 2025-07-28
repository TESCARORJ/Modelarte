using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraMovimentacaoEquipamentoApiService
    {
        private readonly HttpClient _httpClient;

        public ObraMovimentacaoEquipamentoApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MovimentacaoEquipamentoDto>> GetMovimentacoesByObraEquipamentoIdAsync(long obraEquipamentoId)
        {
            var response = await _httpClient.GetAsync($"api/ObraEquipamentoMovimentacao/{obraEquipamentoId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<MovimentacaoEquipamentoDto>>() ?? new List<MovimentacaoEquipamentoDto>();
        }

        public async Task<MovimentacaoEquipamentoDto> CreateMovimentacaoAsync(MovimentacaoEquipamentoDto movimentacao)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ObraEquipamentoMovimentacao", movimentacao);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MovimentacaoEquipamentoDto>();
        }

        // Outros métodos como Update ou Delete podem ser adicionados conforme necessário
    }
}
