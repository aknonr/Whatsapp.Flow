using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook
{
    public class ChangeValue
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }

        [JsonPropertyName("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }

        [JsonPropertyName("wa_id")]
        public string WaId { get; set; }

        [JsonPropertyName("identity_key_hash")]
        public string IdentityKeyHash { get; set; }
    }

    public class Profile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // Adding Status class for message status
    public class Status
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("status")]
        public string StatusType { get; set; } // e.g., "sent", "delivered"

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("recipient_id")]
        public string RecipientId { get; set; }
    }
} 