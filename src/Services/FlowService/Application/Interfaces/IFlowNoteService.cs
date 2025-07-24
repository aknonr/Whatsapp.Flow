using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Application.Interfaces
{
    public interface IFlowNoteService
    {
        Task<FlowNote> AddNoteAsync(string flowId, string content, string createdBy, string color = "#FFD700", int priority = 3, string category = "Genel");
        Task<bool> UpdateNoteAsync(string flowId, string noteId, string content, string updatedBy, string color = null, int? priority = null, string category = null);
        Task<bool> DeleteNoteAsync(string flowId, string noteId);
        Task<List<FlowNote>> GetNotesByFlowIdAsync(string flowId);
        Task<FlowNote> GetNoteByIdAsync(string flowId, string noteId);
        Task<List<FlowNote>> GetNotesByCategoryAsync(string flowId, string category);
        Task<List<FlowNote>> GetNotesByPriorityAsync(string flowId, int priority);
    }
} 