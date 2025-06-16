using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FornecedorImportacaoApiService
{
    private readonly HttpClient _http;

    public FornecedorImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FornecedorDto>> PreviewExcelFornecedoresAsync(Stream stream, string fileName = "fornecedores.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/fornecedores/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos fornecedores.");

        return await response.Content.ReadFromJsonAsync<List<FornecedorDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarFornecedoresAsync(List<FornecedorDto> fornecedores)
    {
        var response = await _http.PostAsJsonAsync("api/fornecedores/importacao/importar", fornecedores);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar fornecedores.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
