using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraItemEtapaPadraoApiService
    {
        private readonly HttpClient _http;
        public ObraItemEtapaPadraoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraItemEtapaPadraoDto>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaPadraoDto>>("api/obraitemetapapadrao") ?? new();

        public async Task<ObraItemEtapaPadraoDto?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ObraItemEtapaPadraoDto>($"api/obraitemetapapadrao/{id}");

        public async Task<List<ObraItemEtapaPadraoDto>> GetByEtapaPadraoIdAsync(long etapaPadraoId) =>
            await _http.GetFromJsonAsync<List<ObraItemEtapaPadraoDto>>($"api/obraitemetapapadrao/por-etapa/{etapaPadraoId}") ?? new();

        // No seu ObraItemEtapaPadraoApiService (no frontend)
        public async Task<ObraItemEtapaPadraoDto> CreateAsync(ObraItemEtapaPadraoDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/obraitemetapapadrao", dto);

            // Se o status for de conflito (409), leia a mensagem e lance uma exceção para a UI tratar
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception(error?.message ?? "Registro duplicado.");
            }

            // Garante que outros erros ainda sejam lançados
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ObraItemEtapaPadraoDto>();
        }


        public async Task UpdateAsync(ObraItemEtapaPadraoDto dto){
            var response = await _http.PutAsJsonAsync($"api/obraitemetapapadrao/{dto.Id}", dto);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erro: {response.StatusCode}");
        }

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraitemetapapadrao/{id}");
    }
}
