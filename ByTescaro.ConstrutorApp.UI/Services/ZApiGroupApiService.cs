using ByTescaro.ConstrutorApp.Application.DTOs;

namespace ByTescaro.ConstrutorApp.UI.Services
{
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

            var allGroups = new List<GroupDto>();
            var page = 1;
            const int pageSize = 100;
            bool hasMorePages;

            do
            {
                var requestUri = $"{baseUrl}/instances/{instanceId}/token/{token}/groups?page={page}&pageSize={pageSize}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("Client-Token", clientToken);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro ao buscar grupos do Z-API na página {page}: {response.ReasonPhrase}. Detalhes: {errorContent}");
                }

                var currentPageGroups = await response.Content.ReadFromJsonAsync<List<GroupDto>>();

                if (currentPageGroups == null || !currentPageGroups.Any())
                {
                    break; // Nenhuma grupo retornado, sair do loop.
                }

                allGroups.AddRange(currentPageGroups);
                hasMorePages = currentPageGroups.Count == pageSize;
                page++;

            } while (hasMorePages);

            return allGroups.Where(g => g.IsGroup).ToList();
        }
    }
}
