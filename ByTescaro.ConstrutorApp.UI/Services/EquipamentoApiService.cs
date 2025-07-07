using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class EquipamentoApiService
{
    private readonly HttpClient _http;

    public EquipamentoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EquipamentoDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/equipamento");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar equipamentos: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<EquipamentoDto>>() ?? new();
    }

    public async Task<EquipamentoDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/equipamento/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<EquipamentoDto>();
    }

    public async Task CreateAsync(EquipamentoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/equipamento", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar equipamento: {response.StatusCode}");
    }

    public async Task UpdateAsync(EquipamentoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/equipamento/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar equipamento: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/equipamento/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir equipamento: {response.StatusCode}");
    }

    //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
    //{
    //    var response = await _http.GetFromJsonAsync<ResumoAlocacaoDto>("api/equipamento/ObterResumoAlocacaoAsync");
    //    return (response?.Alocados ?? 0, response?.NaoAlocados ?? 0);
    //}

    public async Task<bool> NomeExistsAsync(string nome, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) return false;
        string requestUrl = $"api/equipamento/NomeExists?nome={Uri.EscapeDataString(nome)}";
        if (ignoreId.HasValue && ignoreId.Value > 0) requestUrl += $"&ignoreId={ignoreId.Value}";

        var response = await _http.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<bool>();

        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao verificar nome do Equipamento: {response.StatusCode} - {errorContent}");
        return false;
    }

    public async Task<bool> PatrimonioExistsAsync(string patrimonio, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(patrimonio)) return false;
        string requestUrl = $"api/equipamento/PatrimonioExists?patrimonio={Uri.EscapeDataString(patrimonio)}";
        if (ignoreId.HasValue && ignoreId.Value > 0) requestUrl += $"&ignoreId={ignoreId.Value}";

        var response = await _http.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<bool>();

        var errorContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao verificar patrimônio do Equipamento: {response.StatusCode} - {errorContent}");
        return false;
    }
}



