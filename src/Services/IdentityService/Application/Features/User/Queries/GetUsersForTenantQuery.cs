using MediatR;
using System.Collections.Generic;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Queries
{
    public class GetUsersForTenantQuery : IRequest<IEnumerable<UserDto>>
    {
    }
} 