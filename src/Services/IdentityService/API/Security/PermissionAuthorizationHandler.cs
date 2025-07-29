using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;
using System.Linq;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using System.Collections.Generic;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using System;

namespace Whatsapp.Flow.Services.Identity.API.Security
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly ICacheService _cacheService;

        public PermissionAuthorizationHandler(ITenantRoleRepository tenantRoleRepository, ICacheService cacheService)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _cacheService = cacheService;
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

            var cacheKey = $"permissions:{tenantId}:{userId}";
            var userPermissions = await _cacheService.GetAsync<List<Permission>>(cacheKey);

            if (userPermissions == null)
            {
                var permissionsFromDb = await _tenantRoleRepository.GetUserPermissionsAsync(userId, tenantId);
                if (permissionsFromDb == null)
                {
                    context.Fail();
                    return;
                }
                userPermissions = permissionsFromDb.ToList();
                await _cacheService.SetAsync(cacheKey, userPermissions, TimeSpan.FromMinutes(5));
            }


            if (!userPermissions.Any())
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