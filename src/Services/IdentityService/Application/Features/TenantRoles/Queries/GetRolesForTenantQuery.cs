using MediatR;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Queries
{
    public class GetRolesForTenantQuery : IRequest<IEnumerable<TenantRoleDto>>
    {
    }
} 