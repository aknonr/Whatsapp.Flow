using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.Events
{
    public record WhatsappMessageReceivedIntegrationEvent : IntegrationEvent
    {
        public string PhoneNumber { get; init; }
        public string Message { get; init; }
        public string TenantId { get; init; }
        public DateTime MessageTimestamp { get; init; }
        public string MessageId { get; init; }
    }
} 