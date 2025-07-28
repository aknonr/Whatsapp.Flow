using Microsoft.AspNetCore.Authorization;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.API.Security
{
    public class HasRoleAttribute : AuthorizeAttribute
    {
        public HasRoleAttribute(params Role[] roles)
        {
            var allowedRolesAsString = roles.Select(x => x.ToString());
            Roles = string.Join(",", allowedRolesAsString);
        }
    }
} 