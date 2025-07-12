using Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.Services.WebhookService.Application.IntegrationEvents.Events
{
    public record WhatsappMessageReceivedIntegrationEvent : IntegrationEvent
    {
        public WebhookPayload WebhookPayload { get; }

        public WhatsappMessageReceivedIntegrationEvent(WebhookPayload webhookPayload)
        {
            WebhookPayload = webhookPayload;
        }
    }
} 