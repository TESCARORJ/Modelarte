using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http;


namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraApiService
    {
        private readonly HttpClient _http;

        public ObraApiService(HttpClient http) => _http = http;       

        public async Task<List<ObraEtapaDto>> GetEtapasAsync(long obraId)
        {
            var response = await _http.GetAsync($"api/obra/{obraId}/etapas");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ObraEtapaDto>>() ?? new();
        }

        public async Task<List<ObraItemEtapaDto>> GetItensEtapaAsync(long obraEtapaId)
        {
            var response = await _http.GetAsync($"api/obra/itensEtapas/{obraEtapaId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ObraItemEtapaDto>>() ?? new();
        }

        public async Task<List<ObraDto>> GetByProjetoIdAsync(long projetoId)
        {
            var response = await _http.GetAsync($"api/obra/por-projeto/{projetoId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ObraDto>>() ?? new();
        }
        public async Task<List<ObraDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraDto>>("api/obra") ?? new();

        public async Task<ObraDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraDto>($"api/obra/{id}");

        public async Task<ObraDto?> CreateAsync(ObraDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/obra", dto);

            if (response.IsSuccessStatusCode)
            {
                // Lê o DTO do corpo da resposta e o retorna
                return await response.Content.ReadFromJsonAsync<ObraDto>();
            }
            else
            {
                // Lança uma exceção ou retorna nulo para indicar o erro
                // (aqui estamos retornando nulo, mas lançar exceção pode ser melhor)
                return null;
            }
        }

        public async Task UpdateAsync(ObraDto dto) =>
            await _http.PutAsJsonAsync($"api/obra/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obra/{id}");

        public async Task<int> GetProgressoAsync(long id) =>
            await _http.GetFromJsonAsync<int>($"api/obra/{id}/progresso");

        public async Task<List<ObraEtapaDto>> ObterEtapasAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraEtapaDto>>($"api/obra/{obraId}/etapas") ?? new();

        public async Task<List<ObraItemEtapaDto>> ObterItensEtapaAsync(long obraEtapaId) =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaDto>>($"api/obra/itensEtapas/{obraEtapaId}") ?? new();

        public async Task AtualizarConclusaoItemAsync(long itemId, bool concluido) =>
            await _http.PatchAsync($"api/obra/concluirItem/{itemId}", JsonContent.Create(concluido));
    }
}
