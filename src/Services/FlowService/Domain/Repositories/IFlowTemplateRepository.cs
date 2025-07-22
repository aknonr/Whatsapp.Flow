using System.Collections.Generic;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Repositories
{
    public interface IFlowTemplateRepository
    {
        Task<FlowTemplate> GetByIdAsync(string id);
        Task<IEnumerable<FlowTemplate>> GetAllPublicAsync();
        Task<IEnumerable<FlowTemplate>> GetByCategoryAsync(string category);
        Task<IEnumerable<FlowTemplate>> SearchAsync(string searchTerm);
        Task<FlowTemplate> CreateAsync(FlowTemplate template);
        Task<bool> UpdateAsync(string id, FlowTemplate template);
        Task<bool> DeleteAsync(string id);
        Task IncrementUsageCountAsync(string templateId);
    }
} 