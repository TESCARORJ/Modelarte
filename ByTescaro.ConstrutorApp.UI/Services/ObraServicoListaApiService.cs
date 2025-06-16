using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraServicoListaApiService
    {
        private readonly HttpClient _http;

        public ObraServicoListaApiService(HttpClient http) => _http = http;

        public async Task<List<ObraServicoListaDto>> GetPorObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraServicoListaDto>>($"api/obraservicolista/obra/{obraId}") ?? new();

        public async Task<ObraServicoListaDto?> GetPorIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraServicoListaDto>($"api/obraservicolista/{id}");

        public async Task<ObraServicoListaDto?> CreateAsync(ObraServicoListaDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/obraservicolista", dto);
            return await response.Content.ReadFromJsonAsync<ObraServicoListaDto>();
        }


        public async Task UpdateAsync(ObraServicoListaDto dto) =>
            await _http.PutAsJsonAsync($"api/obraservicolista/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraservicolista/{id}");
    }
}
