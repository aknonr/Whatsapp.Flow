using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Application.Interfaces
{
    public interface IWhatsappService
    {
        Task SendMessageAsync(string toPhoneNumber, string messageContent);
        Task SendButtonMessageAsync(string toPhoneNumber, ButtonNode buttonNode);
        Task SendListMessageAsync(string toPhoneNumber, ListMenuNode listMenuNode);
    }
} 