using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> GetByIdAsync(string id);
        Task AddAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
    }
} 