using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events
{
    public record TenantInfoUpdatedIntegrationEvent : IntegrationEvent
    {
        public string TenantId { get; }
        public string PhoneNumber { get; }

        public TenantInfoUpdatedIntegrationEvent(string tenantId, string phoneNumber)
        {
            TenantId = tenantId;
            PhoneNumber = phoneNumber;
        }
    }
} 