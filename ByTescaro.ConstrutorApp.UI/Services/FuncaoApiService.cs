using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FuncaoApiService
{
    private readonly HttpClient _http;

    public FuncaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FuncaoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/funcao");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar funções: {response.StatusCode} - {error}");
        }

        return await response.Content.ReadFromJsonAsync<List<FuncaoDto>>() ?? new();
    }

    public async Task<FuncaoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/funcao/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<FuncaoDto>();
    }

    public async Task CreateAsync(FuncaoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/funcao", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar função: {response.StatusCode}");
    }

    public async Task UpdateAsync(FuncaoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/funcao/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar função: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/funcao/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir função: {response.StatusCode}");
    }
}
