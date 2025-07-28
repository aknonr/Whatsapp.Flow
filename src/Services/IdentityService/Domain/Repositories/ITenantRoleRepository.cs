using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Domain.Repositories
{
    public interface ITenantRoleRepository
    {
        Task<TenantRole> GetByIdAsync(string id);
        Task<List<TenantRole>> GetByTenantIdAsync(string tenantId);
        Task<TenantRole> GetByNameAsync(string tenantId, string roleName);
        Task<TenantRole> AddAsync(TenantRole role);
        Task UpdateAsync(TenantRole role);
        Task DeleteAsync(string id);
        Task<List<Permission>> GetUserPermissionsAsync(string userId, string tenantId);
    }
} 