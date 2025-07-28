using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Subscriptions.Queries
{
    public record GetSubscriptionByTenantIdQuery(string TenantId) : IRequest<SubscriptionDto>;
} 