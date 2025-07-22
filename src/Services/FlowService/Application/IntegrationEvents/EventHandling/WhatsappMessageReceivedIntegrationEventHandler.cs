using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;
using Whatsapp.Flow.Services.Flow.Application.Services;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using Whatsapp.Flow.Services.WebhookService.Application.IntegrationEvents.Events;

namespace Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.EventHandling
{
    public class WhatsappMessageReceivedIntegrationEventHandler : IIntegrationEventHandler<WhatsappMessageReceivedIntegrationEvent>
    {
        private readonly ILogger<WhatsappMessageReceivedIntegrationEventHandler> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly FlowEngine _flowEngine;

        public WhatsappMessageReceivedIntegrationEventHandler(
            ILogger<WhatsappMessageReceivedIntegrationEventHandler> logger,
            ITenantRepository tenantRepository,
            FlowEngine flowEngine)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _flowEngine = flowEngine;
        }

        public async Task Handle(WhatsappMessageReceivedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName}", @event.Id, "FlowService");

            var message = @event.WebhookPayload?.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault()?.Value?.Messages?.FirstOrDefault();
            if (message == null)
            {
                _logger.LogWarning("Event'te işlenecek mesaj bulunamadı. EventId: {IntegrationEventId}", @event.Id);
                return;
            }

            var tenant = await _tenantRepository.GetByPhoneNumberAsync(message.From);
            if (tenant == null)
            {
                _logger.LogWarning("{PhoneNumber} numarasına kayıtlı bir tenant bulunamadı.", message.From);
                return;
            }
            
            // FlowEngine'e tüm mesaj nesnesini gönderiyoruz.
            await _flowEngine.RunAsync(tenant, message);
        }
    }
} 