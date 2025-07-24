using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Application.Services
{
    public class FlowNoteService : IFlowNoteService
    {
        private readonly IFlowRepository _flowRepository;

        public FlowNoteService(IFlowRepository flowRepository)
        {
            _flowRepository = flowRepository;
        }

        public async Task<FlowNote> AddNoteAsync(string flowId, string content, string createdBy, string color = "#FFD700", int priority = 3, string category = "Genel")
        {
            var note = new FlowNote
            {
                Content = content,
                Color = color,
                Priority = priority,
                Category = category,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                Position = new Position { X = 0, Y = 0 } // Varsayılan pozisyon
            };

            return await _flowRepository.AddNoteAsync(flowId, note);
        }

        public async Task<bool> UpdateNoteAsync(string flowId, string noteId, string content, string updatedBy, string color = null, int? priority = null, string category = null)
        {
            var existingNote = await _flowRepository.GetNoteByIdAsync(flowId, noteId);
            if (existingNote == null)
                return false;

            var updatedNote = new FlowNote
            {
                Id = noteId,
                Content = content,
                Color = color ?? existingNote.Color,
                Priority = priority ?? existingNote.Priority,
                Category = category ?? existingNote.Category,
                CreatedBy = existingNote.CreatedBy,
                CreatedAt = existingNote.CreatedAt,
                UpdatedBy = updatedBy,
                UpdatedAt = DateTime.UtcNow,
                Position = existingNote.Position
            };

            return await _flowRepository.UpdateNoteAsync(flowId, noteId, updatedNote);
        }

        public async Task<bool> DeleteNoteAsync(string flowId, string noteId)
        {
            return await _flowRepository.DeleteNoteAsync(flowId, noteId);
        }

        public async Task<List<FlowNote>> GetNotesByFlowIdAsync(string flowId)
        {
            return await _flowRepository.GetNotesByFlowIdAsync(flowId);
        }

        public async Task<FlowNote> GetNoteByIdAsync(string flowId, string noteId)
        {
            return await _flowRepository.GetNoteByIdAsync(flowId, noteId);
        }

        public async Task<List<FlowNote>> GetNotesByCategoryAsync(string flowId, string category)
        {
            var notes = await _flowRepository.GetNotesByFlowIdAsync(flowId);
            return notes.Where(n => n.Category == category).ToList();
        }

        public async Task<List<FlowNote>> GetNotesByPriorityAsync(string flowId, int priority)
        {
            var notes = await _flowRepository.GetNotesByFlowIdAsync(flowId);
            return notes.Where(n => n.Priority == priority).ToList();
        }
    }
} 