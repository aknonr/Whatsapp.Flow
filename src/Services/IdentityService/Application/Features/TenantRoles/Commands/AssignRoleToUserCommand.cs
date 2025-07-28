using MediatR;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class AssignRoleToUserCommand : IRequest
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
} 