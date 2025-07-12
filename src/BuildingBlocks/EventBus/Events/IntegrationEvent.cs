using System;
using System.Text.Json.Serialization;

namespace Whatsapp.Flow.BuildingBlocks.EventBus.Events
{
    public record IntegrationEvent
    {
        [JsonInclude]
        public Guid Id { get; private init; }

        [JsonInclude]
        public DateTime CreationDate { get; private init; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
} 