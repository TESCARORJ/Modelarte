using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Net.Http;

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

    public async Task<bool> CpfCnpjExistsAsync(string cpfCnpj, long? ignoreId = null)
    {
        string requestUrl = $"api/cliente/CpfCnpjExists?cpfCnpj={cpfCnpj}";
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

        string requestUrl = $"api/cliente/TelefonePrincipalExists?telefonePrincipal={telefonePrincipal}";

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

        string requestUrl = $"api/cliente/TelefoneWhatsAppExists?telefoneWhatsApp={telefoneWhatsApp}";

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

        string requestUrl = $"api/cliente/EmailExists?email={email}";

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


