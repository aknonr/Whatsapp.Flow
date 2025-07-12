using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public record CreateTenantCommand(string Name, string PhoneNumber) : IRequest<string>;
} 