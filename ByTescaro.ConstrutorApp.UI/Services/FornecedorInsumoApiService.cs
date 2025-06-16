using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FornecedorInsumoApiService
{
    private readonly HttpClient _http;

    public FornecedorInsumoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FornecedorInsumoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/fornecedorinsumo");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorInsumoDto>>() ?? new();
    }

    public async Task<FornecedorInsumoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/fornecedorinsumo/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<FornecedorInsumoDto>();
    }

    public async Task<List<FornecedorInsumoDto>> GetByFornecedorAsync(long fornecedorId)
    {
        var response = await _http.GetAsync($"api/fornecedorinsumo/por-fornecedor/{fornecedorId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorInsumoDto>>() ?? new();
    }

    public async Task<List<FornecedorInsumoDto>> GetByInsumoAsync(long insumoId)
    {
        var response = await _http.GetAsync($"api/fornecedorinsumo/por-insumo/{insumoId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorInsumoDto>>() ?? new();
    }

    public async Task CreateAsync(FornecedorInsumoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/fornecedorinsumo", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(FornecedorInsumoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/fornecedorinsumo/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro: {response.StatusCode} - {errorContent}");
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/fornecedorinsumo/{id}");
        response.EnsureSuccessStatusCode();
    }
}
