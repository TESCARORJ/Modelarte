using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class FuncionarioImportacaoApiService
{
    private readonly HttpClient _http;

    public FuncionarioImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FuncionarioDto>> PreviewExcelFuncionariosAsync(Stream stream, string fileName = "funcionarios.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/funcionarios/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos funcionarios.");

        return await response.Content.ReadFromJsonAsync<List<FuncionarioDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarFuncionariosAsync(List<FuncionarioDto> funcionarios)
    {
        var response = await _http.PostAsJsonAsync("api/funcionarios/importacao/importar", funcionarios);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar funcionarios.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
