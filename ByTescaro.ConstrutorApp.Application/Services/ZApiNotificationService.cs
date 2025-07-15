using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Infrastructure.Services; // Certifique-se de que este namespace está correto para ZApiSettings
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Text; // Para StringBuilder
using System.Text.Json; // Para JsonSerializer
using System.Linq; // Para .Select
using RestSharp; // <--- NOVO: Importar RestSharp
using System.Threading.Tasks;
using ByTescaro.ConstrutorApp.Domain.Interfaces; // Certificar que está presente

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public class ZApiNotificationService : INotificationService
    {
        private readonly RestClient _restClient; // <--- MUDANÇA: Usar RestClient
        private readonly ZApiSettings _zApiSettings;
        private readonly ILogger<ZApiNotificationService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        // <--- MUDANÇA: Construtor agora recebe RestClient em vez de IHttpClientFactory
        public ZApiNotificationService(IOptions<ZApiSettings> zApiSettings, ILogger<ZApiNotificationService> logger, IUnitOfWork unitOfWork)
        {
            _zApiSettings = zApiSettings.Value;
            _logger = logger;

            // <--- MUDANÇA: Instanciar RestClient com a BaseUrl
            // A BaseUrl deve ser a raiz da API, Ex: "https://api.z-api.io/"
            // Os InstanceId e InstanceToken serão adicionados ao path da requisição.
            _restClient = new RestClient(_zApiSettings.BaseUrl);
            _unitOfWork = unitOfWork;
        }

        private string CleanAndFormatPhoneNumber(string phoneNumber)
        {
            var numeroLimpo = Regex.Replace(phoneNumber, @"[^\d]", "");
            if (numeroLimpo.Length > 0 && !numeroLimpo.StartsWith("55"))
            {
                numeroLimpo = "55" + numeroLimpo;
            }
            return numeroLimpo;
        }

        public async Task SendWhatsAppMessageAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Tentativa de envio de mensagem de texto com telefone ou mensagem vazia.");
                return;
            }

            var numeroLimpo = CleanAndFormatPhoneNumber(phoneNumber);
            // O RestClient.BaseUrl já é "https://api.z-api.io/", então path é /instances/...
            var resource = $"instances/{_zApiSettings.InstanceId}/token/{_zApiSettings.InstanceToken}/send-text";

            var payload = new
            {
                phone = numeroLimpo,
                message = message
            };

            // <--- MUDANÇAS AQUI PARA USAR RestSharp
            var request = new RestRequest(resource, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Client-Token", _zApiSettings.ClientToken);
            request.AddJsonBody(payload); // Adiciona o payload como JSON no corpo da requisição

            _logger.LogInformation("Z-API - Enviando requisição de texto (RestSharp) para URL: {Url}", _restClient.BuildUri(request));

            try
            {
                var response = await _restClient.ExecuteAsync(request);

                _logger.LogInformation("Z-API - Resposta texto (RestSharp). Status: {StatusCode}, Resposta Conteúdo: {ResponseContent}", response.StatusCode, response.Content);

                if (!response.IsSuccessful) // Verifica se a requisição foi bem-sucedida (status 2xx)
                {
                    _logger.LogError("Z-API retornou um erro ao enviar texto (RestSharp). Status: {StatusCode}, Resposta: {ErrorContent}", response.StatusCode, response.Content);
                    // Você pode optar por lançar uma exceção mais específica aqui se quiser
                    throw new ApplicationException($"Erro Z-API ao enviar texto: Status {response.StatusCode}, Conteúdo: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de texto para Z-API (RestSharp).");
                throw;
            }
        }

        public async Task SendWhatsAppMessageWithButtonsAsync(string phoneNumber, string message, List<string> buttonTexts, string customId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message) || buttonTexts == null || !buttonTexts.Any())
            {
                _logger.LogWarning("Tentativa de envio de mensagem com botões com dados incompletos.");
                return;
            }

            var numeroLimpo = CleanAndFormatPhoneNumber(phoneNumber);
            var resource = $"instances/{_zApiSettings.InstanceId}/token/{_zApiSettings.InstanceToken}/send-button-list";

            var buttons = buttonTexts.Select((text, index) => new
            {
                id = customId != null ? $"{customId}_btn_{index + 1}" : (index + 1).ToString(),
                label = text
            }).ToList();

            var payload = new
            {
                phone = numeroLimpo,
                message = message,
                buttonList = new
                {
                    buttons = buttons
                }
            };

            // Serializa o payload JSON completo que será enviado
            string jsonPayloadString = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation("Z-API - Payload JSON final enviado para '{Resource}' (RestSharp, AddParameter):\n{JsonPayload}", resource, jsonPayloadString);

            var request = new RestRequest(resource, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Client-Token", _zApiSettings.ClientToken);

            // 🚨 FOCO DA REFATORAÇÃO: Usar AddParameter com o JSON stringificado
            // A documentação do Z-API mostra explicitamente:
            // request.AddParameter("undefined", "{ \"phone\": ... }", ParameterType.RequestBody);
            // Isso significa que "undefined" é o nome do parâmetro (que é ignorado, mas deve ser passado)
            // e o segundo argumento é a string JSON literal do corpo da requisição.
            request.AddParameter("undefined", jsonPayloadString, ParameterType.RequestBody);

            _logger.LogInformation("Z-API - Enviando requisição com botões (send-button-list, RestSharp, AddParameter) para URL: {Url}", _restClient.BuildUri(request));
            _logger.LogInformation("Z-API - Telefone: {Phone}, CustomId (base do botão): {CustomId}", numeroLimpo, customId);
            _logger.LogInformation("Z-API - Header 'Client-Token' está presente.");

            try
            {
                var response = await _restClient.ExecuteAsync(request);

                _logger.LogInformation("Z-API - Resposta (send-button-list, RestSharp, AddParameter). Status: {StatusCode}, Resposta Conteúdo: {ResponseContent}", response.StatusCode, response.Content);

                if (!response.IsSuccessful)
                {
                    _logger.LogError("Z-API - Erro de API ao enviar botões (RestSharp, AddParameter). Status: {StatusCode}, Resposta: {ErrorContent}", response.StatusCode, response.Content);
                    throw new ApplicationException($"Erro Z-API ao enviar botões: Status {response.StatusCode}, Conteúdo: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Z-API - Erro GERAL ao enviar mensagem com botões para Z-API (RestSharp, AddParameter).");
                throw;
            }
        }

        // NOVO: Implementação do método SendNotificationToAllActiveUsers
        // Este método será responsável por buscar os usuários participantes e enviar a notificação.
        // A lógica de "participantes" é arbitrária aqui, assumindo que são todos os usuários com telefone.
        // Você precisará refinar a lógica de seleção de usuários com base nas suas regras de negócio.
        public async Task SendNotificationToAllActiveUsers(string subject, string message)
        {
            _logger.LogInformation("Z-API: Iniciando envio de notificação para todos os usuários participantes.");

            // Combina assunto e mensagem para o conteúdo do WhatsApp
            var fullMessage = string.IsNullOrWhiteSpace(subject) ? message : $"{subject}\n\n{message}";

            // Busca todos os usuários. Adicione um filtro para "usuários ativos" ou "participantes" se necessário.
            // Para este exemplo, assumimos que Usuario possui um campo Telefone e Ativo.
            // Você pode precisar de um método específico no UsuarioRepository, como GetActiveUsersWithPhoneNumberAsync()
            var allUsers = await _unitOfWork.UsuarioRepository.GetAllAsync(); //

            // Filtra usuários que possuem um número de telefone e que estão ativos (se houver o campo Ativo na entidade Usuario)
            // Assumindo que Usuario tem uma propriedade Telefone (string) e Ativo (bool)
            var usersToNotify = allUsers.Where(u => !string.IsNullOrWhiteSpace(u.TelefoneWhatsApp) && u.Ativo).ToList(); //

            if (!usersToNotify.Any()) //
            {
                _logger.LogWarning("Z-API: Nenhum usuário participante encontrado para enviar a notificação."); //
                return;
            }

            foreach (var user in usersToNotify) //
            {
                try
                {
                    _logger.LogInformation("Z-API: Enviando notificação para o usuário {UserName} ({PhoneNumber}).", user.Nome, user.TelefoneWhatsApp); //
                    await SendWhatsAppMessageAsync(user.TelefoneWhatsApp, fullMessage); //
                }
                catch (Exception ex)
                {
                    // Loga o erro, mas não interrompe o envio para os outros usuários
                    _logger.LogError(ex, "Z-API: Falha ao enviar notificação para o usuário {UserName} ({PhoneNumber}).", user.Nome, user.TelefoneWhatsApp); //
                }
            }

            _logger.LogInformation("Z-API: Concluído o processo de envio de notificações para usuários participantes."); //
        }

        // NOVO: Implementação do método SendNotificationAsync
        // Este método é mais genérico e pode ser usado para enviar uma notificação para um usuário específico.
        public async Task SendNotificationAsync(long userId, string subject, string message)
        {
            _logger.LogInformation("Z-API: Tentando enviar notificação para o usuário ID: {UserId}.", userId); //

            // Combina assunto e mensagem para o conteúdo do WhatsApp
            var fullMessage = string.IsNullOrWhiteSpace(subject) ? message : $"{subject}\n\n{message}"; //

            // Busca o usuário pelo ID
            var user = await _unitOfWork.UsuarioRepository.GetByIdAsync(userId); //

            if (user == null) //
            {
                _logger.LogWarning("Z-API: Usuário com ID {UserId} não encontrado para envio de notificação.", userId); //
                return;
            }

            if (string.IsNullOrWhiteSpace(user.TelefoneWhatsApp)) //
            {
                _logger.LogWarning("Z-API: Usuário {UserName} (ID: {UserId}) não possui número de telefone cadastrado para envio de notificação.", user.Nome, userId); //
                return;
            }

            // Supondo que a entidade Usuario tenha uma propriedade 'Ativo'
            if (!user.Ativo) //
            {
                _logger.LogWarning("Z-API: Usuário {UserName} (ID: {UserId}) está inativo e não receberá notificações.", user.Nome, userId); //
                return;
            }

            try
            {
                _logger.LogInformation("Z-API: Enviando notificação para o usuário {UserName} ({PhoneNumber}).", user.Nome, user.TelefoneWhatsApp); //
                await SendWhatsAppMessageAsync(user.TelefoneWhatsApp, fullMessage); //
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Z-API: Falha ao enviar notificação para o usuário {UserName} (ID: {UserId}).", user.Nome, userId); //
                throw; // Re-lança a exceção para que a camada superior possa lidar com ela
            }
        }
    }
}