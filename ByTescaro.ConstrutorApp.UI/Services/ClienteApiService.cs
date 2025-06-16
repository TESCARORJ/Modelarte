using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class ClienteApiService
{
    private readonly HttpClient _http;

    public ClienteApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ClienteDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/cliente");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar clientes: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<ClienteDto>>() ?? new();
    }

    public async Task<ClienteDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/cliente/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ClienteDto>();
    }

    public async Task CreateAsync(ClienteDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/cliente", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar cliente: {response.StatusCode}");
    }

    public async Task UpdateAsync(ClienteDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/cliente/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar cliente: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/cliente/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir cliente: {response.StatusCode}");
    }
}


