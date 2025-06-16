using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraChecklistApiService
    {
        private readonly HttpClient _http;

        public ObraChecklistApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ObraEtapaDto>> GetEtapasAsync(long obraId)
            => await _http.GetFromJsonAsync<List<ObraEtapaDto>>($"api/obras/{obraId}/checklist");

        public async Task SaveEtapasAsync(long obraId, List<ObraEtapaDto> etapas)
        {
            var response = await _http.PutAsJsonAsync($"api/obras/{obraId}/checklist", etapas);
            response.EnsureSuccessStatusCode();
        }
    }

}
