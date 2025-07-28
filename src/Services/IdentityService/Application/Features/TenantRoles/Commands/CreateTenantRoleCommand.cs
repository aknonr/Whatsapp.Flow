using MediatR;
using System.Collections.Generic;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class CreateTenantRoleCommand : IRequest<string>
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<Permission> Permissions { get; set; }
    }
} 