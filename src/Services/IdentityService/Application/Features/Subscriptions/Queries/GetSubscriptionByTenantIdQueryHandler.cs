using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries
{
    public class GetSubscriptionByTenantIdQueryHandler : IRequestHandler<GetSubscriptionByTenantIdQuery, SubscriptionDto>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptionByTenantIdQueryHandler(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionDto> Handle(GetSubscriptionByTenantIdQuery request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByTenantIdAsync(request.TenantId);

            if (subscription == null)
            {
                throw new Exception("Subscription not found for the specified tenant."); // Daha spesifik bir exception kullanılabilir
            }

            return new SubscriptionDto
            {
                Id = subscription.Id,
                TenantId = subscription.TenantId,
                Plan = subscription.Plan.ToString(),
                Status = subscription.Status.ToString(),
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                TrialEndDate = subscription.TrialEndDate,
                MaxUsers = subscription.MaxUsers,
                MaxFlows = subscription.MaxFlows,
                MaxMessagesPerMonth = subscription.MaxMessagesPerMonth,
                MaxPhoneNumbers = subscription.MaxPhoneNumbers,
                CurrentUsers = subscription.CurrentUsers,
                CurrentFlows = subscription.CurrentFlows,
                MessagesThisMonth = subscription.MessagesThisMonth,
                CurrentPhoneNumbers = subscription.CurrentPhoneNumbers,
                MonthlyPrice = subscription.MonthlyPrice,
                Currency = subscription.Currency,
                NextPaymentDate = subscription.NextPaymentDate
            };
        }
    }
} 