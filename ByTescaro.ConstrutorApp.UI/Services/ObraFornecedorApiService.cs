using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ObraFornecedorApiService
    {
        private readonly HttpClient _http;
        public ObraFornecedorApiService(HttpClient http) => _http = http;

        public async Task<List<ObraFornecedorDto>> GetByObraIdAsync(long obraId) =>
            await _http.GetFromJsonAsync<List<ObraFornecedorDto>>($"api/obrafornecedor/{obraId}") ?? new();

        public async Task<List<FornecedorDto>> GetFornecedoresDisponiveisAsync(long obraId) =>
          await _http.GetFromJsonAsync<List<FornecedorDto>>($"api/ObraFornecedor/disponiveis/{obraId}") ?? new();

        public async Task<List<FornecedorDto>> GetFornecedoresTotalDisponiveisAsync() =>
         await _http.GetFromJsonAsync<List<FornecedorDto>>($"api/ObraFornecedor/total-disponiveis") ?? new();
         public async Task<List<FornecedorDto>> GetFornecedoresAlocadosAsync() =>
         await _http.GetFromJsonAsync<List<FornecedorDto>>($"api/ObraFornecedor/total-alocados") ?? new();

        public async Task CreateAsync(ObraFornecedorDto dto) =>
            await _http.PostAsJsonAsync("api/obrafornecedor", dto);

        public async Task UpdateAsync(ObraFornecedorDto dto) =>
            await _http.PutAsJsonAsync($"api/obrafornecedor/{dto.Id}", dto);

        public async Task DeleteAsync(long id) =>
            await _http.DeleteAsync($"api/obrafornecedor/{id}");
    }
}
