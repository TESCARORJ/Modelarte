using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FornecedorServicoApiService
{
    private readonly HttpClient _http;

    public FornecedorServicoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FornecedorServicoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/FornecedorServico");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorServicoDto>>() ?? new();
    }

    public async Task<FornecedorServicoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/FornecedorServico/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<FornecedorServicoDto>();
    }

    public async Task<List<FornecedorServicoDto>> GetByFornecedorAsync(long fornecedorId)
    {
        var response = await _http.GetAsync($"api/FornecedorServico/por-fornecedor/{fornecedorId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorServicoDto>>() ?? new();
    }

    public async Task<List<FornecedorServicoDto>> GetByServicoAsync(long servicoId)
    {
        var response = await _http.GetAsync($"api/FornecedorServico/por-servico/{servicoId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<FornecedorServicoDto>>() ?? new();
    }

    public async Task CreateAsync(FornecedorServicoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/FornecedorServico", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(FornecedorServicoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/FornecedorServico/{dto.Id}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/FornecedorServico/{id}");
        response.EnsureSuccessStatusCode();
    }
}
