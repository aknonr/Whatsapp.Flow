using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using System;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries
{
    public class GetSubscriptionByTenantIdQueryHandler : IRequestHandler<GetSubscriptionByTenantIdQuery, SubscriptionDto>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICacheService _cacheService;

        public GetSubscriptionByTenantIdQueryHandler(ISubscriptionRepository subscriptionRepository, ICacheService cacheService)
        {
            _subscriptionRepository = subscriptionRepository;
            _cacheService = cacheService;
        }

        public async Task<SubscriptionDto> Handle(GetSubscriptionByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"subscription:{request.TenantId}";
            
            var cachedSubscription = await _cacheService.GetAsync<SubscriptionDto>(cacheKey);
            if (cachedSubscription != null)
            {
                return cachedSubscription;
            }

            var subscription = await _subscriptionRepository.GetByTenantIdAsync(request.TenantId);

            if (subscription == null)
            {
                throw new NotFoundException(nameof(subscription), request.TenantId);
            }

            var subscriptionDto = new SubscriptionDto
            {
                Id = subscription.Id,
                TenantId = subscription.TenantId,
                Plan = subscription.Plan,
                Status = subscription.Status,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                TrialEndDate = subscription.TrialEndDate,
                Limits = new SubscriptionLimitsDto 
                {
                    MaxUsers = subscription.Limits.MaxUsers,
                    MaxFlows = subscription.Limits.MaxFlows,
                    MaxMessagesPerMonth = subscription.Limits.MaxMessagesPerMonth,
                    MaxPhoneNumbers = subscription.Limits.MaxPhoneNumbers
                },
                CurrentUsage = new SubscriptionUsageDto
                {
                    Users = subscription.CurrentUsage.Users,
                    Flows = subscription.CurrentUsage.Flows,
                    MessagesSentThisMonth = subscription.CurrentUsage.MessagesSentThisMonth,
                    PhoneNumbers = subscription.CurrentUsage.PhoneNumbers
                }
            };
            
            await _cacheService.SetAsync(cacheKey, subscriptionDto, TimeSpan.FromMinutes(15));

            return subscriptionDto;
        }
    }
} 