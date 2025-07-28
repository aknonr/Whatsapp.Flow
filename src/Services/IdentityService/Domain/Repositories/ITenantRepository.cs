using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task AddAsync(Tenant tenant);
        Task<Tenant> GetByIdAsync(string id);
        Task UpdateAsync(Tenant tenant);
        Task DeleteAsync(string id); // Bu Hard Delete için kalabilir
        Task SoftDeleteAsync(string id, string userId);
    }
} 