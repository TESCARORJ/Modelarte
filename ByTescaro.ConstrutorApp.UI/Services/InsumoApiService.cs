using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class InsumoApiService
{
    private readonly HttpClient _http;

    public InsumoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<InsumoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/insumo");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar insumos: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<InsumoDto>>() ?? new();
    }

    public async Task<InsumoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/insumo/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<InsumoDto>();
    }

    public async Task CreateAsync(InsumoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/insumo", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar insumo: {response.StatusCode}");
    }

    public async Task UpdateAsync(InsumoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/insumo/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar insumo: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/insumo/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir insumo: {response.StatusCode}");
    }

    public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) return false;
        string requestUrl = $"api/insumo/NomeExists?nome={Uri.EscapeDataString(nome)}";
        if (ignoreId.HasValue && ignoreId.Value > 0) requestUrl += $"&ignoreId={ignoreId.Value}";

        var response = await _http.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<bool>();

        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao verificar nome do Insumo: {response.StatusCode} - {errorContent}");
        return false;
    }
}


