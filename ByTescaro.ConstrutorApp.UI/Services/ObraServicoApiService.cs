using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraServicoApiService
    {
        private readonly HttpClient _http;

        public ObraServicoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraServicoDto>> GetByListaIdAsync(long listaId) =>
            await _http.GetFromJsonAsync<List<ObraServicoDto>>($"api/obraservico/lista/{listaId}") ?? new();

        public async Task<List<ServicoDto>> GetServicosDisponiveisAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ServicoDto>>($"api/obraservico/disponiveis/{obraId}") ?? new();

        public async Task CreateAsync(ObraServicoDto dto) =>
            await _http.PostAsJsonAsync("api/obraservico", dto);

        public async Task UpdateAsync(ObraServicoDto dto) =>
            await _http.PutAsJsonAsync($"api/obraservico/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraservico/{id}");
    }
}
