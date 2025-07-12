using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Domain.Repositories
{
    public interface IFlowStateRepository
    {
        Task<FlowState?> GetByUserPhoneAsync(string userPhoneNumber);
        Task<FlowState> CreateAsync(FlowState flowState);
        Task UpdateAsync(FlowState flowState);
    }
} 