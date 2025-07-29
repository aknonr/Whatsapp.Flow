using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Commands
{
    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand>
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICacheService _cacheService;

        public UpdateSubscriptionCommandHandler(ISubscriptionRepository subscriptionRepository, ICacheService cacheService)
        {
            _subscriptionRepository = subscriptionRepository;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByTenantIdAsync(request.TenantId);

            if (subscription == null)
            {
                throw new NotFoundException("Subscription", request.TenantId);
            }

            // Sadece dolu gelen alanları güncelle
            if (request.Plan.HasValue)
            {
                subscription.Plan = request.Plan.Value;
            }

            if (request.Status.HasValue)
            {
                subscription.Status = request.Status.Value;
            }

            if (request.EndDate.HasValue)
            {
                subscription.EndDate = request.EndDate.Value;
            }

            await _subscriptionRepository.UpdateAsync(subscription);

            // Cache'i temizle
            var cacheKey = $"subscription:{request.TenantId}";
            await _cacheService.RemoveAsync(cacheKey);
        }
    }
} 