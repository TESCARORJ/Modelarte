using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraItemEtapaApiService
    {
        private readonly HttpClient _http;
        public ObraItemEtapaApiService(HttpClient http) => _http = http;

        public async Task<List<ObraItemEtapaDto>> GetByEtapaIdAsync(long etapaId) =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaDto>>($"api/obraitensetapa/{etapaId}") ?? new();

        public async Task AtualizarConclusaoAsync(long itemId, bool concluido) =>
            await _http.PatchAsync($"api/obraitensetapa/concluir/{itemId}", JsonContent.Create(concluido));

        public async Task CreateAsync(ObraItemEtapaDto dto) =>
            await _http.PostAsJsonAsync("api/obraitensetapa", dto);

        public async Task UpdateAsync(ObraItemEtapaDto dto) =>
            await _http.PutAsJsonAsync($"api/obraitensetapa/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraitensetapa/{id}");
    }
}
