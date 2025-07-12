using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Whatsapp.Flow.BuildingBlocks.Common.Domain.Entities
{
    public abstract partial class BaseEntity
    {
        protected BaseEntity()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
} 