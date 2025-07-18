using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class OrcamentoApiService
    {
        private readonly HttpClient _http;

        public OrcamentoApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<OrcamentoDto?> GetByObraAsync(long obraId)
        {
            return await _http.GetFromJsonAsync<OrcamentoDto>($"api/orcamento/obra/{obraId}");
        }

        public async Task CreateAsync(OrcamentoDto dto)
        {
            await _http.PostAsJsonAsync("api/orcamento", dto);
        }
        public async Task UpdateAsync(OrcamentoDto dto)
        {
            await _http.PutAsJsonAsync($"api/orcamento/{dto.Id}", dto);
        }

        public async Task<List<OrcamentoDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<OrcamentoDto>>("api/orcamento");
        }

        public async Task DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"api/orcamento/{id}");
            response.EnsureSuccessStatusCode();
        }

    }

}
