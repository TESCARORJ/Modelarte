using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FuncionarioApiService
{
    private readonly HttpClient _http;

    public FuncionarioApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FuncionarioDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/funcionario");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar funcionarios: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<FuncionarioDto>>() ?? new();
    }

    public async Task<List<FuncionarioDto>> GetAllAtivosAsync()
    {
        var response = await _http.GetAsync("api/funcionario/ativos");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar funcionarios: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<FuncionarioDto>>() ?? new();
    }

    public async Task<FuncionarioDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/funcionario/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<FuncionarioDto>();
    }

    public async Task CreateAsync(FuncionarioDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/funcionario", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao criar funcionario: {response.StatusCode}");
    }

    public async Task UpdateAsync(FuncionarioDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/funcionario/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar funcionario: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/funcionario/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir funcionario: {response.StatusCode}");
    }

    //public async Task<(int Alocados, int NaoAlocados)> ObterResumoAlocacaoAsync()
    //{
    //    var response = await _http.GetFromJsonAsync<ResumoAlocacaoDto>("api/funcionario/ObterResumoAlocacaoAsync");
    //    return (response?.Alocados ?? 0, response?.NaoAlocados ?? 0);
    //}


    public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
    {
        string requestUrl = $"api/funcionario/CpfCnpjExists?cpfCnpj={cpfCnpj}";
        if (ignoreId.HasValue && ignoreId.Value > 0)
        {
            requestUrl += $"&ignoreId={ignoreId.Value}";
        }

        var response = await _http.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao verificar CPF/CNPJ: {response.StatusCode} - {errorContent}");
            return false;
        }
    }

    public async Task<bool> TelefonePrincipalExistsAsync(string telefonePrincipal, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(telefonePrincipal))
        {
            return false;
        }

        string requestUrl = $"api/funcionario/TelefonePrincipalExists?telefonePrincipal={telefonePrincipal}";

        if (ignoreId.HasValue && ignoreId.Value > 0)
        {
            requestUrl += $"&ignoreId={ignoreId.Value}";
        }

        var response = await _http.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao verificar Telefone Principal: {response.StatusCode} - {errorContent}");
            return false;
        }
    }

    public async Task<bool> TelefoneWhatsAppExistsAsync(string telefoneWhatsApp, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(telefoneWhatsApp))
        {
            return false;
        }

        string requestUrl = $"api/funcionario/TelefoneWhatsAppExists?telefoneWhatsApp={telefoneWhatsApp}";

        if (ignoreId.HasValue && ignoreId.Value > 0)
        {
            requestUrl += $"&ignoreId={ignoreId.Value}";
        }

        var response = await _http.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao verificar Telefone WhatsApp: {response.StatusCode} - {errorContent}");
            return false;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, long? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string requestUrl = $"api/funcionario/EmailExists?email={email}";

        if (ignoreId.HasValue && ignoreId.Value > 0)
        {
            requestUrl += $"&ignoreId={ignoreId.Value}";
        }

        var response = await _http.GetAsync(requestUrl);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao verificar Email: {response.StatusCode} - {errorContent}");
            return false;
        }
    }
}





