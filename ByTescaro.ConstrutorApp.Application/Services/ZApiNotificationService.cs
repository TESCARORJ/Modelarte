using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ZApiNotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ZApiSettings _zApiSettings;
        private readonly ILogger<ZApiNotificationService> _logger;

        public ZApiNotificationService(IHttpClientFactory httpClientFactory, IOptions<ZApiSettings> zApiSettings, ILogger<ZApiNotificationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ZApiClient");
            _zApiSettings = zApiSettings.Value;
            _logger = logger;
        }

        public async Task SendWhatsAppMessageAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Tentativa de envio de mensagem com telefone ou mensagem vazia.");
                return;
            }

            var requestUrl = $"{_zApiSettings.InstanceId}/token/{_zApiSettings.InstanceToken}/send-text";

            var numeroLimpo = Regex.Replace(phoneNumber, @"[^\d]", "");
            if (numeroLimpo.Length > 0 && !numeroLimpo.StartsWith("55"))
            {
                numeroLimpo = "55" + numeroLimpo;
            }

            var payload = new
            {
                phone = numeroLimpo,
                message = message
            };

            try
            {
                // ================== NOVO TESTE DE DEPURAÇÃO ==================
                // Vamos construir a requisição manualmente para garantir que o header está sendo enviado.

                // 1. Criar o conteúdo JSON
                using var jsonContent = JsonContent.Create(payload);

                // 2. Criar a mensagem de requisição
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);

                // 3. Adicionar o conteúdo à mensagem
                requestMessage.Content = jsonContent;

                // 4. ADICIONAR O HEADER DIRETAMENTE NA REQUISIÇÃO (aqui é a parte mais importante do teste)
                requestMessage.Headers.Add("Client-Token", _zApiSettings.ClientToken);

                _logger.LogInformation("Enviando requisição MANUAL para {Url} com header 'Client-Token' e telefone {Phone}", requestUrl, numeroLimpo);

                // 5. Enviar a requisição construída manualmente
                var response = await _httpClient.SendAsync(requestMessage);
                // =============================================================


                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Z-API retornou um erro (TESTE MANUAL). Status: {StatusCode}, Resposta: {ErrorContent}", response.StatusCode, errorContent);
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro GERAL ao enviar mensagem para Z-API.");
                throw;
            }
        }
    }
}