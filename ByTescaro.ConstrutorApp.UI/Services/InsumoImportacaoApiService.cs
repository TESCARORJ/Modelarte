using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class InsumoImportacaoApiService
{
    private readonly HttpClient _http;

    public InsumoImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<InsumoDto>> PreviewExcelInsumosAsync(Stream stream, string fileName = "insumos.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/insumos/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos insumos.");

        return await response.Content.ReadFromJsonAsync<List<InsumoDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarInsumosAsync(List<InsumoDto> insumos)
    {
        var response = await _http.PostAsJsonAsync("api/insumos/importacao/importar", insumos);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar insumos.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
