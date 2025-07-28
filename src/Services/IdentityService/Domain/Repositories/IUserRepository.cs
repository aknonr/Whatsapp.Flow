using Whatsapp.Flow.Services.Identity.Domain.Entities;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        
        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);
        
        Task UpdateAsync(User user);
        
        Task DeleteAsync(string id);
    }
} 