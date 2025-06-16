using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class PerfilUsuarioApiService
{
    private readonly HttpClient _http;

    public PerfilUsuarioApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PerfilUsuarioDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/perfilUsuario");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar perfilUsuarios: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<PerfilUsuarioDto>>() ?? new();
    }

    public async Task<PerfilUsuarioDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/perfilUsuario/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<PerfilUsuarioDto>();
    }

    public async Task CreateAsync(PerfilUsuarioDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/perfilUsuario", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar perfilUsuario: {response.StatusCode}");
    }

    public async Task UpdateAsync(PerfilUsuarioDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/perfilUsuario/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar perfilUsuario: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/perfilUsuario/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir perfilUsuario: {response.StatusCode}");
    }

    public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
    {
        var response = await _http.GetFromJsonAsync<ResumoAlocacaoDto>("api/perfilUsuario/ObterResumoAlocacaoAsync");
        return (response?.Alocados ?? 0, response?.NaoAlocados ?? 0);
    }

}


