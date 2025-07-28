using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> GetByIdAsync(string id);
        Task<Subscription> GetByTenantIdAsync(string tenantId);
        Task<Subscription> AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task<bool> CheckLimitAsync(string tenantId, string limitType);
        Task IncrementUsageAsync(string tenantId, string usageType, int amount = 1);
    }
} 