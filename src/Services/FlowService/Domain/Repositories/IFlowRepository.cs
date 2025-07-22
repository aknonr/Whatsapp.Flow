using System.Collections.Generic;
using System.Threading.Tasks;

namespace Whatsapp.Flow.Services.Flow.Domain.Repositories
{
    public interface IFlowRepository
    {
        Task<Entities.Flow> GetByIdAsync(string id);
        Task<IEnumerable<Entities.Flow>> GetByTenantIdAsync(string tenantId);
        Task<Entities.Flow> GetActiveFlowByTenantIdAsync(string tenantId);
        Task<IEnumerable<Entities.Flow>> GetActiveFlowsByTenantIdAsync(string tenantId);
        Task<Entities.Flow> GetFlowByTriggerKeywordAsync(string tenantId, string keyword);
        Task<IEnumerable<Entities.Flow>> GetFlowsByCategoryAsync(string tenantId, string category);
        Task<Entities.Flow> AddNewAsync(Entities.Flow flow);
        Task<bool> UpdateAsync(string id, Entities.Flow flow);
        Task<bool> DeleteAsync(string id);
        Task<bool> SetActiveStatusAsync(string id, bool isActive);
        Task<int> GetFlowCountByTenantAsync(string tenantId);
    }
} 