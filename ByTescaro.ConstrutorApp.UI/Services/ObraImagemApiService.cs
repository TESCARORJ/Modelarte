using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraImagemApiService
    {
        private readonly HttpClient _http;
        public ObraImagemApiService(HttpClient http) => _http = http;

        public async Task<List<ObraImagemDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraImagemDto>>($"api/obraimagem/{obraId}") ?? new();

        public async Task CreateAsync(ObraImagemDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/obraimagem", dto);
            response.EnsureSuccessStatusCode();
            // Se o controller retornar o DTO atualizado, você pode deserializá-lo aqui
            // var createdDto = await response.Content.ReadFromJsonAsync<ObraImagemDto>();
            // return createdDto;
        }

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraimagem/{id}");
    }
}
