namespace ByTescaro.ConstrutorApp.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendWhatsAppMessageAsync(string phoneNumber, string message);
    }
}
