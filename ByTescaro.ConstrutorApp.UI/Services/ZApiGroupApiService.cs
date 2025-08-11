using ByTescaro.ConstrutorApp.Application.DTOs;
using System.Text.Json;

namespace ByTescaro.ConstrutorApp.UI.Services
{
    public class ZApiGroupResponse
    {
        public List<GroupDto> Value { get; set; }
    }

    public class ZApiGroupApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ZApiGroupApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<GroupDto>> GetGroupsAsync()
        {
            // Obtém a URL da API da configuração
            var baseUrl = _configuration["ZApiSettings:BaseUrl"];
            var instanceId = _configuration["ZApiSettings:InstanceId"];
            var token = _configuration["ZApiSettings:InstanceToken"];
            var clientToken = _configuration["ZApiSettings:ClientToken"];

            var requestUri = $"{baseUrl}/instances/{instanceId}/token/{token}/groups?page=1&pageSize=100"; // Ajuste o pageSize conforme necessário

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Client-Token", clientToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ZApiGroupResponse>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var groups = apiResponse?.Value;
                return groups?.Where(g => g.IsGroup).ToList() ?? new List<GroupDto>();
            }
            else
            {
                // Logar ou tratar o erro
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao buscar grupos do Z-API: {response.ReasonPhrase}. Detalhes: {errorContent}");
            }
        }
    }
}
