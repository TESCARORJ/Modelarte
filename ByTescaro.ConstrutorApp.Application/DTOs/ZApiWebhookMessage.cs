// ByTescaro.ConstrutorApp.Application.DTOs/ZApiWebhookMessage.cs
using System.Text.Json.Serialization;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class ZApiWebhookMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } // Ex: "message", "buttonReply", "ack" (para status de envio)

        [JsonPropertyName("from")]
        public string From { get; set; } // Número de telefone do remetente (ex: "5521999999999@c.us")

        [JsonPropertyName("to")]
        public string To { get; set; } // Seu número/instância que recebeu a mensagem

        [JsonPropertyName("id")]
        public string Id { get; set; } // ID da mensagem no WhatsApp/Z-API

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } // Timestamp Unix da mensagem

        [JsonPropertyName("content")]
        public string Content { get; set; } // Conteúdo da mensagem. Para botões, geralmente é o texto do botão.

        [JsonPropertyName("customId")] // Este é o campo que conterá o 'custom_id' que você enviou
        public string CustomId { get; set; }

        // Outras propriedades que o webhook pode enviar, dependendo do tipo de evento
        // [JsonPropertyName("chatId")]
        // public string ChatId { get; set; }

        // Se o Z-API enviar um objeto de "button" ou "interactive"
        // [JsonPropertyName("button")]
        // public WebhookButton Button { get; set; }
    }

    // Exemplo se o Z-API aninhar os detalhes do botão
    // public class WebhookButton
    // {
    //     [JsonPropertyName("id")]
    //     public string Id { get; set; }
    //     [JsonPropertyName("text")]
    //     public string Text { get; set; }
    // }
}