using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task AddAsync(Tenant tenant);
        Task<Tenant> GetByIdAsync(string id);
    }
} 