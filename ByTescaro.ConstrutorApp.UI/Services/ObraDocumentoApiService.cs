using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraDocumentoApiService
    {
        private readonly HttpClient _http;
        public ObraDocumentoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraDocumentoDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraDocumentoDto>>($"api/obradocumento/{obraId}") ?? new();

        public async Task CreateAsync(ObraDocumentoDto dto) =>
            await _http.PostAsJsonAsync("api/obradocumento", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obradocumento/{id}");
    }
}
