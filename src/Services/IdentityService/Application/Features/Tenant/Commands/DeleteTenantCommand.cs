using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public record DeleteTenantCommand(string Id) : IRequest;
} 