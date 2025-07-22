using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class FlowState
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TenantId { get; set; }
        public string FlowId { get; set; }
        public string UserPhoneNumber { get; set; }
        public string CurrentNodeId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime LastInteractionAt { get; set; } = DateTime.UtcNow;
        
        // Flow değişkenleri - kullanıcıya özel
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        
        // Konuşma geçmişi
        public List<ConversationEntry> ConversationHistory { get; set; } = new List<ConversationEntry>();
        
        // Bekleme durumu (WaitNode için)
        public WaitingState WaitingState { get; set; }
    }

    public class ConversationEntry
    {
        public string NodeId { get; set; }
        public string NodeType { get; set; }
        public MessageDirection Direction { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public enum MessageDirection
    {
        Incoming,  // Kullanıcıdan gelen
        Outgoing   // Bot tarafından gönderilen
    }

    public class WaitingState
    {
        public bool IsWaiting { get; set; }
        public DateTime? WaitUntil { get; set; }
        public string WaitingNodeId { get; set; }
        public string WaitType { get; set; }
    }
} 