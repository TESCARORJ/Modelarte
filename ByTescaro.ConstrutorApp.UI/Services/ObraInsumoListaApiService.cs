using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraInsumoListaApiService
    {
        private readonly HttpClient _http;

        public ObraInsumoListaApiService(HttpClient http) => _http = http;

        public async Task<List<ObraInsumoListaDto>> GetPorObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraInsumoListaDto>>($"api/obrainsumolista/obra/{obraId}") ?? new();

        public async Task<ObraInsumoListaDto?> GetPorIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraInsumoListaDto>($"api/obrainsumolista/{id}");

        public async Task<ObraInsumoListaDto?> CreateAsync(ObraInsumoListaDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/obrainsumolista", dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar lista de insumos: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<ObraInsumoListaDto>();
        }


        public async Task UpdateAsync(ObraInsumoListaDto dto) =>
            await _http.PutAsJsonAsync($"api/obrainsumolista/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obrainsumolista/{id}");
    }
}
