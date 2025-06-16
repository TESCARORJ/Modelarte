using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class ServicoImportacaoApiService
{
    private readonly HttpClient _http;

    public ServicoImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ServicoDto>> PreviewExcelServicosAsync(Stream stream, string fileName = "servicos.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/servicos/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos servicos.");

        return await response.Content.ReadFromJsonAsync<List<ServicoDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarServicosAsync(List<ServicoDto> servicos)
    {
        var response = await _http.PostAsJsonAsync("api/servicos/importacao/importar", servicos);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar servicos.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
