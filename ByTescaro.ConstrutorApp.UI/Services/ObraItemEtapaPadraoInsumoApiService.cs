using Azure;
using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraItemEtapaPadraoInsumoApiService
    {
        private readonly HttpClient _http;
        public ObraItemEtapaPadraoInsumoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraItemEtapaPadraoInsumoDto>> GetByItemIdAsync(long itemId)
        {
            var response = await _http.GetAsync($"api/ObraItemEtapaPadraoInsumo/{itemId}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro: {response.StatusCode}");

            var data = await response.Content.ReadFromJsonAsync<List<ObraItemEtapaPadraoInsumoDto>>();
            return data ?? new();
        }

        public async Task<List<InsumoDto>> GetRelacionadosPorObraAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<InsumoDto>>($"api/ObraItemEtapaPadraoInsumo/por-obra/{obraId}") ?? new();

        public async Task CreateAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/ObraItemEtapaPadraoInsumo", dto);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro: {response.StatusCode}");
        }


        public async Task UpdateAsync(ObraItemEtapaPadraoInsumoDto dto)
        {
          var response =  await _http.PutAsJsonAsync($"api/ObraItemEtapaPadraoInsumo/{dto.Id}", dto);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro: {response.StatusCode}");
        }
        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/ObraItemEtapaPadraoInsumo/{id}");
    }

}
