using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class ClienteImportacaoApiService
{
    private readonly HttpClient _http;

    public ClienteImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ClienteDto>> PreviewExcelClientesAsync(Stream stream, string fileName = "clientes.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/clientes/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos clientes.");

        return await response.Content.ReadFromJsonAsync<List<ClienteDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarClientesAsync(List<ClienteDto> clientes)
    {
        var response = await _http.PostAsJsonAsync("api/clientes/importacao/importar", clientes);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar clientes.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
