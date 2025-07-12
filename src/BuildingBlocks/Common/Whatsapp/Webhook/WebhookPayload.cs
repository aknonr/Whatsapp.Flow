using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook
{
    public class WebhookPayload
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("entry")]
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("changes")]
        public List<Change> Changes { get; set; }
    }

    public class Change
    {
        [JsonPropertyName("value")]
        public ChangeValue Value { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }
    }
} 