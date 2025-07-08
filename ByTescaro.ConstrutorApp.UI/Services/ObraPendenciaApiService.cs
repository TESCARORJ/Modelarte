using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraPendenciaApiService
    {
        private readonly HttpClient _http;
        public ObraPendenciaApiService(HttpClient http) => _http = http;

        public async Task<List<ObraPendenciaDto>> GetByObraIdAsync(long obraId) =>
           await _http.GetFromJsonAsync<List<ObraPendenciaDto>>($"api/obrapendencia/{obraId}") ?? new();

        public async Task<List<ObraPendenciaDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraPendenciaDto>>("api/obrapendencia") ?? new();

        public async Task<ObraPendenciaDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraPendenciaDto>($"api/obrapendencia/{id}");


        public async Task CreateAsync(ObraPendenciaDto dto) =>
            await _http.PostAsJsonAsync("api/obrapendencia", dto);

        public async Task UpdateAsync(ObraPendenciaDto dto) =>
            await _http.PutAsJsonAsync($"api/obrapendencia/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obrapendencia/{id}");
    }
}
