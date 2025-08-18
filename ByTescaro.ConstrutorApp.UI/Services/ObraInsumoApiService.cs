using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraInsumoApiService
    {
        private readonly HttpClient _http;

        public ObraInsumoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraInsumoDto>> GetByListaIdAsync(long listaId) =>
            await _http.GetFromJsonAsync<List<ObraInsumoDto>>($"api/obrainsumo/lista/{listaId}") ?? new();

        public async Task<List<InsumoDto>> GetInsumosDisponiveisAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<InsumoDto>>($"api/obrainsumo/disponiveis/{obraId}") ?? new();

        public async Task<List<InsumoDto>> GetInsumosAsync() =>
          await _http.GetFromJsonAsync<List<InsumoDto>>($"api/obrainsumo/all") ?? new();

        public async Task CreateAsync(ObraInsumoDto dto) =>
            await _http.PostAsJsonAsync("api/obrainsumo", dto);

        public async Task UpdateAsync(ObraInsumoDto dto) =>
            await _http.PutAsJsonAsync($"api/obrainsumo/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obrainsumo/{id}");

        public async Task<List<InsumoDto>> GetInsumosPadraoRelacionadosAsync(long obraId)
        {
            return await _http.GetFromJsonAsync<List<InsumoDto>>($"api/obrainsumo/padrao-relacionados/{obraId}") ?? new();
        }

    }
}
