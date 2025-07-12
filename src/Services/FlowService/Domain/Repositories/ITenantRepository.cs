using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> GetByPhoneNumberAsync(string phoneNumber);
        Task UpsertAsync(Tenant tenant);
    }
} 