using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using TenantEntity = Whatsapp.Flow.Services.Identity.Domain.Entities.Tenant;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, string>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IEventBus _eventBus;

        public CreateTenantCommandHandler(ITenantRepository tenantRepository, IEventBus eventBus)
        {
            _tenantRepository = tenantRepository;
            _eventBus = eventBus;
        }

        public async Task<string> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = new TenantEntity
            {
                Name = request.Name,
                PhoneNumber = request.PhoneNumber
            };

            await _tenantRepository.AddAsync(tenant);

            var tenantInfoEvent = new TenantInfoUpdatedIntegrationEvent(tenant.Id, tenant.PhoneNumber);
            _eventBus.Publish(tenantInfoEvent);

            return tenant.Id;
        }
    }
} 