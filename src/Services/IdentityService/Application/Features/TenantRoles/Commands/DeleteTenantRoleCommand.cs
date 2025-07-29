using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public record DeleteTenantRoleCommand(string Id) : IRequest;
} 