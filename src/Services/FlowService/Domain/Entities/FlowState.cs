using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
    }
} 