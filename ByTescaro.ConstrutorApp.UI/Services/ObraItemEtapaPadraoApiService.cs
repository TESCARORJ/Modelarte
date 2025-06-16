using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraItemEtapaPadraoApiService
    {
        private readonly HttpClient _http;
        public ObraItemEtapaPadraoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraItemEtapaPadraoDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaPadraoDto>>("api/obraitemetapapadrao") ?? new();

        public async Task<ObraItemEtapaPadraoDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraItemEtapaPadraoDto>($"api/obraitemetapapadrao/{id}");

        public async Task<List<ObraItemEtapaPadraoDto>> GetByEtapaPadraoIdAsync(long etapaPadraoId) =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaPadraoDto>>($"api/obraitemetapapadrao/por-etapa/{etapaPadraoId}") ?? new();

        public async Task CreateAsync(ObraItemEtapaPadraoDto dto) =>
            await _http.PostAsJsonAsync("api/obraitemetapapadrao", dto);

        public async Task UpdateAsync(ObraItemEtapaPadraoDto dto) =>
            await _http.PutAsJsonAsync($"api/obraitemetapapadrao/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraitemetapapadrao/{id}");
    }
}
