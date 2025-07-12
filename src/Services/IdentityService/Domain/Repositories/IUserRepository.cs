using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        
        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);

        // Gelecekte ihtiyaç duyulabilecek diğer metodlar buraya eklenebilir.
        // Örneğin: Task UpdateAsync(User user);
        // Örneğin: Task DeleteAsync(string id);
    }
} 