using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.Services.Flow.Domain.Repositories;
using Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;
using Whatsapp.Flow.Services.Flow.Domain.Entities;
using Whatsapp.Flow.BuildingBlocks.EventBus.Events;

namespace Whatsapp.Flow.Services.FlowService.Application.IntegrationEvents.EventHandling
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
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName}", @event.Id, "FlowService");

            var tenant = new Tenant
            {
                Id = @event.TenantId,
                PhoneNumber = @event.PhoneNumber
            };

            await _tenantRepository.UpsertAsync(tenant);

            _logger.LogInformation("----- Tenant info replicated for TenantId: {TenantId}", @event.TenantId);
        }
    }
} 