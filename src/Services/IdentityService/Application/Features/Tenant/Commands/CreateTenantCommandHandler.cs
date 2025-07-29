using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.BuildingBlocks.EventBus;
using Whatsapp.Flow.BuildingBlocks.EventBus.Abstractions;
using Whatsapp.Flow.Services.Identity.Application.IntegrationEvents.Events;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;


namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, string>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEventBus _eventBus;

        public CreateTenantCommandHandler(ITenantRepository tenantRepository, ISubscriptionRepository subscriptionRepository, IEventBus eventBus)
        {
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subscriptionRepository;
            _eventBus = eventBus;
        }

        public async Task<string> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = new Domain.Entities.Tenant
            {
                Name = request.Name,
                CompanyName = request.CompanyName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = TenantStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);

            // Yeni Tenant için deneme aboneliği oluştur
            var trialSubscription = new Subscription
            {
                TenantId = tenant.Id,
                Plan = SubscriptionPlan.Trial,
                Status = SubscriptionStatus.Trial,
                StartDate = DateTime.UtcNow,
                TrialEndDate = DateTime.UtcNow.AddDays(14), // 14 günlük deneme
                MaxUsers = 2,
                MaxFlows = 5,
                MaxMessagesPerMonth = 500,
                MaxPhoneNumbers = 1
            };

            await _subscriptionRepository.AddAsync(trialSubscription);

            // Publish integration event
            var phoneNumbers = tenant.PhoneNumbers?.Select(p => p.DisplayPhoneNumber).ToList() ?? new List<string>();
            var @event = new TenantInfoUpdatedIntegrationEvent(tenant.Id, tenant.Name, phoneNumbers);
            
            _eventBus.Publish(@event);

            return tenant.Id;
        }
    }
} 