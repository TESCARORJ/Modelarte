using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services;

public class EquipamentoImportacaoApiService
{
    private readonly HttpClient _http;

    public EquipamentoImportacaoApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<EquipamentoDto>> PreviewExcelEquipamentosAsync(Stream stream, string fileName = "equipamentos.xlsx")
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("api/equipamentos/importacao/preview", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao carregar prévia dos equipamentos.");

        return await response.Content.ReadFromJsonAsync<List<EquipamentoDto>>() ?? new();
    }

    public async Task<List<ErroImportacaoDto>> ImportarEquipamentosAsync(List<EquipamentoDto> equipamentos)
    {
        var response = await _http.PostAsJsonAsync("api/equipamentos/importacao/importar", equipamentos);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao importar equipamentos.");

        return await response.Content.ReadFromJsonAsync<List<ErroImportacaoDto>>() ?? new();
    }
}
