using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FornecedorApiService
{
    private readonly HttpClient _http;

    public FornecedorApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FornecedorDto>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/fornecedor");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao buscar fornecedores: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<List<FornecedorDto>>() ?? new();
    }

    public async Task<FornecedorDto?> GetByIdAsync(long id)
    {
        var response = await _http.GetAsync($"api/fornecedor/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<FornecedorDto>();
    }

    public async Task CreateAsync(FornecedorDto dto)
    {
        Console.WriteLine("POST para fornecedor iniciado");

        var response = await _http.PostAsJsonAsync("api/fornecedor", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar fornecedor: {response.StatusCode} - {error}");
            throw new Exception($"Erro ao criar fornecedor: {response.StatusCode} - {error}");
        }

        Console.WriteLine("POST para fornecedor concluído com sucesso");
    }


    public async Task UpdateAsync(FornecedorDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/fornecedor/{dto.Id}", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao atualizar fornecedor: {response.StatusCode}");
    }

    public async Task DeleteAsync(long id)
    {
        var response = await _http.DeleteAsync($"api/fornecedor/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao excluir fornecedor: {response.StatusCode}");
    }

    public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
    {
        string requestUrl = $"api/fornecedor/CpfCnpjExists?cpfCnpj={cpfCnpj}";
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

        string requestUrl = $"api/fornecedor/TelefonePrincipalExists?telefonePrincipal={telefonePrincipal}";

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

        string requestUrl = $"api/fornecedor/TelefoneWhatsAppExists?telefoneWhatsApp={telefoneWhatsApp}";

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

        string requestUrl = $"api/fornecedor/EmailExists?email={email}";

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


