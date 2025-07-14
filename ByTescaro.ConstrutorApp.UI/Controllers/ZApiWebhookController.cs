// ByTescaro.ConstrutorApp.Api.Controllers/ZApiWebhookController.cs
using Microsoft.AspNetCore.Mvc;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Application.DTOs; // Onde ZApiWebhookMessage está
using System.Threading.Tasks;
using System;
using System.Text.Json; // Para logar o payload
using Microsoft.Extensions.Logging; // Para ILogger

namespace ByTescaro.ConstrutorApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZApiWebhookController : ControllerBase
    {
        private readonly IAgendaService _agendaService;
        private readonly ILogger<ZApiWebhookController> _logger;

        public ZApiWebhookController(IAgendaService agendaService, ILogger<ZApiWebhookController> logger)
        {
            _agendaService = agendaService;
            _logger = logger;
        }

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] ZApiWebhookMessage message)
        {
            // Log do payload completo recebido para depuração
            _logger.LogInformation("Webhook Z-API recebido: {MessagePayload}", JsonSerializer.Serialize(message));

            try
            {
                await _agendaService.ProcessarRespostaWebhookZApiAsync(message);
                // Sempre retorne 200 OK para o Z-API, mesmo em caso de erro lógico,
                // para evitar que o Z-API tente reenviar o webhook.
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Z-API.");
                // Logue o erro, mas ainda retorne 200 OK para o Z-API.
                return Ok();
            }
        }
    }
}