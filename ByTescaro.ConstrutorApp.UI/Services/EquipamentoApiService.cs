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

}


