using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http.Json;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class ProjetoApiService
{
    private readonly HttpClient _http;

    public ProjetoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProjetoListDto>> GetAllListAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoListDto>>("api/projeto/list") ?? new();  
    public async Task<List<ProjetoDto>> GetAllAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto") ?? new();

    public async Task<List<ProjetoDto>> GetAllAgendadosAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto/projetosAgendados") ?? new();

    public async Task<List<ProjetoDto>> GetAllEmAndamentoAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto/projetosEmAndamento") ?? new();

    public async Task<List<ProjetoDto>> GetAllCanceladosAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto/projetosCancelados") ?? new();

    public async Task<List<ProjetoDto>> GetAllConcluidosAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto/projetosConcluidos") ?? new();
     public async Task<List<ProjetoDto>> GetAllPausadoAsync() =>
        await _http.GetFromJsonAsync<List<ProjetoDto>>("api/projeto/projetosPausados") ?? new();

    public async Task<ProjetoDto?> GetByIdAsync(long id) =>
        await _http.GetFromJsonAsync<ProjetoDto>($"api/projeto/{id}");

    public async Task<ProjetoDto?> CreateAsync(ProjetoDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/projeto", dto);
        return await response.Content.ReadFromJsonAsync<ProjetoDto>();
    }


    public async Task UpdateAsync(ProjetoDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/projeto/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao atualizar projeto: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/projeto/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ObraEtapaDto>> ObterEtapasAsync(long obraId)
    {
        var response = await _http.GetAsync($"api/obra/{obraId}/etapas");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ObraEtapaDto>>() ?? new();
    }


    public async Task<List<ObraItemEtapaDto>> ObterItensEtapaAsync(long obraEtapaId)
    {
        var response = await _http.GetAsync($"api/obra/itensEtapas/{obraEtapaId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ObraItemEtapaDto>>() ?? new();
    }

    public async Task<int> GetProgressoProjetoAsync(long projetoId)
    {
        var response = await _http.GetAsync($"api/projeto/{projetoId}/progresso");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task AtualizarConclusaoItemAsync(long itemId, bool concluido)
    {
        var response = await _http.PatchAsJsonAsync($"api/projeto/{itemId}/concluirItemEtapaProjeto", concluido);
        response.EnsureSuccessStatusCode();
    }
}
