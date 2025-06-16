using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraEtapaPadraoApiService
    {
        private readonly HttpClient _http;
        public ObraEtapaPadraoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraEtapaPadraoDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraEtapaPadraoDto>>("api/obraetapapadrao") ?? new();

        public async Task<ObraEtapaPadraoDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraEtapaPadraoDto>($"api/obraetapapadrao/{id}");

        public async Task CreateAsync(ObraEtapaPadraoDto dto) =>
            await _http.PostAsJsonAsync("api/obraetapapadrao", dto);

        public async Task UpdateAsync(ObraEtapaPadraoDto dto) =>
            await _http.PutAsJsonAsync($"api/obraetapapadrao/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraetapapadrao/{id}");
    }
}
