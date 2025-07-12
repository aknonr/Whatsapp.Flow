using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Flow.Application.Interfaces
{
    public interface IWhatsappService
    {
        Task SendMessageAsync(string toPhoneNumber, string messageContent);
        // Gelecekte eklenecek: SendListMessageAsync, SendImageMessageAsync vb.
    }
} 