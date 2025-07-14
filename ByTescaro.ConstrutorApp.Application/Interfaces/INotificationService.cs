namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendWhatsAppMessageAsync(string phoneNumber, string message);
        Task SendWhatsAppMessageWithButtonsAsync(string phoneNumber, string message, List<string> buttonTexts, string customId = null);

    }
}
