namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendWhatsAppMessageAsync(string phoneNumber, string message);
        Task SendWhatsAppMessageWithButtonsAsync(string phoneNumber, string message, List<string> buttonTexts, string customId = null);
        Task SendNotificationToAllActiveUsers(string subject, string message);
        Task SendNotificationAsync(long userId, string subject, string message);
        Task SendWhatsAppDocumentAsync(string recipient, byte[] documentBytes, string fileName, string caption = null, bool isGroup = false);


    }
}
