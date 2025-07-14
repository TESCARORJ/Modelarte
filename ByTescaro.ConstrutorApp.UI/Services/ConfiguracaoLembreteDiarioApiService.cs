// ByTescaro.ConstrutorApp.UI.Services/ConfiguracaoLembreteDiarioApiService.cs
using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ConfiguracaoLembreteDiarioApiService
    {
        private readonly HttpClient _http;

        public ConfiguracaoLembreteDiarioApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ConfiguracaoLembreteDiarioDto>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/ConfiguracaoLembreteDiario");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao buscar configurações de lembrete diário: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<List<ConfiguracaoLembreteDiarioDto>>() ?? new();
        }

        public async Task<ConfiguracaoLembreteDiarioDto?> GetByIdAsync(long id)
        {
            var response = await _http.GetAsync($"api/ConfiguracaoLembreteDiario/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Se a API retornar NotFound (404), o ReadFromJsonAsync retornaria null, mas é bom ter o log
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao buscar configuração de lembrete diário por ID: {response.StatusCode} - {errorContent}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ConfiguracaoLembreteDiarioDto>();
        }

        public async Task<ConfiguracaoLembreteDiarioDto> CreateAsync(CriarConfiguracaoLembreteDiarioRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/ConfiguracaoLembreteDiario", request);

            response.EnsureSuccessStatusCode(); // Lança exceção para status codes de erro

            return await response.Content.ReadFromJsonAsync<ConfiguracaoLembreteDiarioDto>();
        }

        public async Task UpdateAsync(AtualizarConfiguracaoLembreteDiarioRequest request)
        {
            var response = await _http.PutAsJsonAsync($"api/ConfiguracaoLembreteDiario/{request.Id}", request);

            response.EnsureSuccessStatusCode(); // Lança exceção para status codes de erro
        }

        public async Task DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"api/ConfiguracaoLembreteDiario/{id}");

            response.EnsureSuccessStatusCode(); // Lança exceção para status codes de erro
        }
    }
}