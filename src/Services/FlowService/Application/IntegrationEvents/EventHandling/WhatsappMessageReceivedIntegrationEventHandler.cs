using Microsoft.Extensions.Logging;
using System;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;
using Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.Events;
using Whatsapp.Flow.Services.Flow.Application.Interfaces;
using Whatsapp.Flow.Services.Flow.Application.Services;
using Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook;
using Whatsapp.Flow.Services.Flow.Domain.Entities;

namespace Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.EventHandling
{
    public class WhatsappMessageReceivedIntegrationEventHandler : IIntegrationEventHandler<WhatsappMessageReceivedIntegrationEvent>
    {
        private readonly ILogger<WhatsappMessageReceivedIntegrationEventHandler> _logger;
        private readonly FlowEngine _flowEngine;

        public WhatsappMessageReceivedIntegrationEventHandler(
            ILogger<WhatsappMessageReceivedIntegrationEventHandler> logger,
            FlowEngine flowEngine)
        {
            _logger = logger;
            _flowEngine = flowEngine;
        }

        public async Task Handle(WhatsappMessageReceivedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling WhatsappMessageReceivedIntegrationEvent for phone: {PhoneNumber}, tenant: {TenantId}", 
                @event.PhoneNumber, @event.TenantId);
            
            try
            {
                // Message nesnesini oluştur
                var message = new Message
                {
                    From = @event.PhoneNumber,
                    Text = new Text { Body = @event.Message },
                    Type = "text",
                    Id = @event.MessageId,
                    Timestamp = @event.MessageTimestamp.ToString()
                };

                // Tenant bilgisini al (gerçek implementasyonda repository'den alınacak)
                var tenant = new Tenant { Id = @event.TenantId };

                // Flow engine'i tetikle
                await _flowEngine.RunAsync(tenant, message);
                
                _logger.LogInformation("Successfully processed message for phone: {PhoneNumber}", @event.PhoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing WhatsappMessageReceivedIntegrationEvent for phone: {PhoneNumber}", 
                    @event.PhoneNumber);
                throw;
            }
        }
    }
} 