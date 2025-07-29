using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;
using Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;

namespace Whatsapp.Flow.Services.Flow.Application.IntegrationEvents.EventHandling
{
    public class TenantInfoUpdatedIntegrationEventHandler : IIntegrationEventHandler<TenantInfoUpdatedIntegrationEvent>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<TenantInfoUpdatedIntegrationEventHandler> _logger;

        public TenantInfoUpdatedIntegrationEventHandler(ITenantRepository tenantRepository, ILogger<TenantInfoUpdatedIntegrationEventHandler> logger)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
        }

        public async Task Handle(TenantInfoUpdatedIntegrationEvent @event)
        {
            _logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            var tenant = await _tenantRepository.GetByIdAsync(@event.TenantId);

            if (tenant == null)
            {
                // Create a new Tenant entity specific to FlowService
                tenant = new Tenant(@event.TenantId, @event.Name, @event.PhoneNumbers);
                await _tenantRepository.AddAsync(tenant);
            }
            else
            {
                // Update existing tenant info
                tenant.UpdateInfo(@event.Name, @event.PhoneNumbers);
                await _tenantRepository.UpdateAsync(tenant);
            }
        }
    }
} 