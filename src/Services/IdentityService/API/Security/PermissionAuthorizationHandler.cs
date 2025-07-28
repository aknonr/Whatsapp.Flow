using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using System.Linq;

namespace Whatsapp.Flow.Services.Identity.API.Security
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;

        public PermissionAuthorizationHandler(ITenantRoleRepository tenantRoleRepository)
        {
            _tenantRoleRepository = tenantRoleRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tenantId = context.User.FindFirstValue("tenantId");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                context.Fail();
                return;
            }

            var userPermissions = await _tenantRoleRepository.GetUserPermissionsAsync(userId, tenantId);

            if (userPermissions == null || !userPermissions.Any())
            {
                context.Fail();
                return;
            }

            // Gerekli izni, formatına göre ayır: "resource.action" -> "roles.create"
            var requiredPermissionParts = requirement.Permission.Split('.', 2);
            if (requiredPermissionParts.Length != 2)
            {
                context.Fail(); // Geçersiz izin formatı
                return;
            }

            var requiredResource = requiredPermissionParts[0];
            var requiredAction = requiredPermissionParts[1];
            
            // Kullanıcının izinleri içinde ilgili kaynağı ve eylemi kontrol et
            var hasPermission = userPermissions
                .Any(p => p.Resource.Equals(requiredResource, System.StringComparison.OrdinalIgnoreCase) &&
                          (p.Actions.Contains("*") || p.Actions.Contains(requiredAction, System.StringComparer.OrdinalIgnoreCase)));
            
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
} 