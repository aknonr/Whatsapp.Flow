using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Queries
{
    public class GetRolesForTenantQueryHandler : IRequestHandler<GetRolesForTenantQuery, IEnumerable<TenantRoleDto>>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetRolesForTenantQueryHandler(ITenantRoleRepository tenantRoleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<TenantRoleDto>> Handle(GetRolesForTenantQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or tenant information is missing.");
            }

            var roles = await _tenantRoleRepository.GetByTenantIdAsync(tenantId);

            return roles.Select(role => new TenantRoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName,
                Description = role.Description,
                Permissions = role.Permissions,
                IsSystemRole = role.IsSystemRole,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy
            });
        }
    }
} 