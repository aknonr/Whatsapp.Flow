using MediatR;
 
namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries
{
    public record GetTenantByIdQuery(string Id) : IRequest<TenantDto>;
} 