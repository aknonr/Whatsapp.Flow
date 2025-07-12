using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities
{
    public class BaseMessageEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string From { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
    }
} 