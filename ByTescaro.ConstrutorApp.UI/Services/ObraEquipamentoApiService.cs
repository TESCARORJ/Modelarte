using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraEquipamentoApiService
    {
        private readonly HttpClient _http;
        public ObraEquipamentoApiService(HttpClient http) => _http = http;

        public async Task<List<ObraEquipamentoDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraEquipamentoDto>>($"api/obraequipamento/{obraId}") ?? new();

        public async Task<List<EquipamentoDto>> GetEquipamentosDisponiveisAsync(long obraId) =>
    await _http.GetFromJsonAsync<List<EquipamentoDto>>($"api/obraequipamento/disponiveis/{obraId}") ?? new();

        public async Task<List<EquipamentoDto>> GetEquipamentosTotalDisponiveisAsync() =>
      await _http.GetFromJsonAsync<List<EquipamentoDto>>($"api/ObraEquipamento/total-disponiveis") ?? new();
        public async Task<List<EquipamentoDto>> GetEquipamentosAlocadosAsync() =>
        await _http.GetFromJsonAsync<List<EquipamentoDto>>($"api/ObraEquipamento/total-alocados") ?? new();

        public async Task CreateAsync(ObraEquipamentoDto dto) =>
            await _http.PostAsJsonAsync("api/obraequipamento", dto);

        public async Task UpdateAsync(ObraEquipamentoDto dto) =>
            await _http.PutAsJsonAsync($"api/obraequipamento/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obraequipamento/{id}");
    

        public async Task MoverEquipamentoAsync(MovimentacaoEquipamentoDto dto) =>
            await _http.PostAsJsonAsync("api/obraequipamento/mover", dto);
    }
}
