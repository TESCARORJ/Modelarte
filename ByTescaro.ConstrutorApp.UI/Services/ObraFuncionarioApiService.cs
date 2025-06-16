using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraFuncionarioApiService
    {
        private readonly HttpClient _http;
        public ObraFuncionarioApiService(HttpClient http) => _http = http;

        public async Task<List<ObraFuncionarioDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraFuncionarioDto>>($"api/obrafuncionario/{obraId}") ?? new();

        public async Task<List<FuncionarioDto>> GetFuncionariosDisponiveisAsync(long obraId) =>
          await _http.GetFromJsonAsync<List<FuncionarioDto>>($"api/ObraFuncionario/disponiveis/{obraId}") ?? new();

        public async Task<List<FuncionarioDto>> GetFuncionariosTotalDisponiveisAsync() =>
         await _http.GetFromJsonAsync<List<FuncionarioDto>>($"api/ObraFuncionario/total-disponiveis") ?? new();
         public async Task<List<FuncionarioDto>> GetFuncionariosAlocadosAsync() =>
         await _http.GetFromJsonAsync<List<FuncionarioDto>>($"api/ObraFuncionario/total-alocados") ?? new();

        public async Task CreateAsync(ObraFuncionarioDto dto) =>
            await _http.PostAsJsonAsync("api/obrafuncionario", dto);

        public async Task UpdateAsync(ObraFuncionarioDto dto) =>
            await _http.PutAsJsonAsync($"api/obrafuncionario/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obrafuncionario/{id}");
    }
}
