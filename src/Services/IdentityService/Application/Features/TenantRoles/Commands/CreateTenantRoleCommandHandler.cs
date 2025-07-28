using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class CreateTenantRoleCommandHandler : IRequestHandler<CreateTenantRoleCommand, string>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateTenantRoleCommandHandler(ITenantRoleRepository tenantRoleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(CreateTenantRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or tenant information is missing.");
            }

            var existingRole = await _tenantRoleRepository.GetByNameAsync(tenantId, request.RoleName);
            if (existingRole != null)
            {
                throw new InvalidOperationException($"Role with name '{request.RoleName}' already exists in this tenant.");
            }
            
            var newRole = new TenantRole
            {
                TenantId = tenantId,
                RoleName = request.RoleName,
                Description = request.Description,
                Permissions = request.Permissions ?? new(),
                IsSystemRole = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var createdRole = await _tenantRoleRepository.AddAsync(newRole);
            return createdRole.Id;
        }
    }
} 