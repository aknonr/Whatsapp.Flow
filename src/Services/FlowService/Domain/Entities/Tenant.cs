using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Whatsapp.Flow.Services.Flow.Domain.Entities
{
    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // Bu Id, IdentityService'teki Tenant'ın Id'si olacak.

        public string PhoneNumber { get; set; }
    }
} 