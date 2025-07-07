using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class ServicoApiService
{
    private readonly HttpClient _http;

    public ServicoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ServicoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/servico");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar servicos: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<ServicoDto>>() ?? new();
    }

    public async Task<ServicoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/servico/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ServicoDto>();
    }

    public async Task CreateAsync(ServicoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/servico", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar servico: {response.StatusCode}");
    }

    public async Task UpdateAsync(ServicoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/servico/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar servico: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/servico/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir servico: {response.StatusCode}");
    }

    public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) return false;
        string requestUrl = $"api/servico/NomeExists?nome={Uri.EscapeDataString(nome)}";
        if (ignoreId.HasValue && ignoreId.Value > 0) requestUrl += $"&ignoreId={ignoreId.Value}";

        var response = await _http.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<bool>();

        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao verificar nome do Serviço: {response.StatusCode} - {errorContent}");
        return false;
    }
}



