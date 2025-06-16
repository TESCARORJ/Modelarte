using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraRetrabalhoApiService
    {
        private readonly HttpClient _http;
        public ObraRetrabalhoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraRetrabalhoDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraRetrabalhoDto>>("api/obraretrabalho") ?? new();

        public async Task<ObraRetrabalhoDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraRetrabalhoDto>($"api/obraretrabalho/{id}");


        public async Task CreateAsync(ObraRetrabalhoDto dto) =>
            await _http.PostAsJsonAsync("api/obraretrabalho", dto);

        public async Task UpdateAsync(ObraRetrabalhoDto dto) =>
            await _http.PutAsJsonAsync($"api/obraretrabalho/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraretrabalho/{id}");
    }
}
