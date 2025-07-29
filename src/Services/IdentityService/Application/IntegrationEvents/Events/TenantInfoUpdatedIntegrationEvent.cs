using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events
{
    public record TenantInfoUpdatedIntegrationEvent(
        string TenantId, 
        string Name, 
        List<string> PhoneNumbers) : IntegrationEvent;
} 