using ByTescaro.ConstrutorApp.Application.DTOs;


namespace ByTescaro.ConstrutorApp.UI.Services;

public class CepService
{
    private readonly HttpClient _httpClient;

    public CepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<EnderecoDto?> BuscarAsync(string cep)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<EnderecoDto>($"https://viacep.com.br/ws/{cep}/json/");
            return response;
        }
        catch
        {
            return null;
        }
    }
}


