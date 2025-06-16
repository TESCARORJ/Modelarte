using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraEtapaApiService
    {
        private readonly HttpClient _http;
        public ObraEtapaApiService(HttpClient http) => _http = http;

        public async Task<List<ObraEtapaDto>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/ObraEtapa");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao buscar etapas de obra: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<ObraEtapaDto>>() ?? new();
        }
        public async Task<List<ObraEtapaDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraEtapaDto>>($"api/obraetapa/{obraId}") ?? new();

        public async Task<ObraEtapaDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraEtapaDto>($"api/obraetapa/{id}");

        public async Task CreateAsync(ObraEtapaDto dto) =>
            await _http.PostAsJsonAsync("api/obraetapa", dto);

        public async Task UpdateAsync(ObraEtapaDto dto) =>
            await _http.PutAsJsonAsync($"api/obraetapa/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraetapa/{id}");
    }
}
