using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class OrcamentoItemApiService
    {
        private readonly HttpClient _http;

        public OrcamentoItemApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task CreateAsync(OrcamentoItemDto dto)
        {
            await _http.PostAsJsonAsync("api/orcamentoitem", dto);
        }

        public async Task UpdateAsync(OrcamentoItemDto dto)
        {
            await _http.PutAsJsonAsync($"api/orcamentoitem/{dto.Id}", dto);
        }

        public async Task DeleteAsync(long id)
        {
            await _http.DeleteAsync($"api/orcamentoitem/{id}");
        }
    }

}
