using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Application.Interfaces;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;

        public AssignRoleToUserCommandHandler(
            IUserRepository userRepository, 
            ITenantRoleRepository tenantRoleRepository, 
            IHttpContextAccessor httpContextAccessor, 
            ICacheService cacheService)
        {
            _userRepository = userRepository;
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
        }

        public async Task Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User's tenant could not be verified.");
            }

            var userToAssign = await _userRepository.GetByIdAsync(request.UserId);
            var roleToAssign = await _tenantRoleRepository.GetByIdAsync(request.RoleId);

            if (userToAssign == null || userToAssign.TenantId != tenantId)
            {
                throw new NotFoundException("User", request.UserId);
            }

            if (roleToAssign == null || roleToAssign.TenantId != tenantId)
            {
                throw new NotFoundException("TenantRole", request.RoleId);
            }

            if (!userToAssign.TenantRoleIds.Contains(request.RoleId))
            {
                userToAssign.TenantRoleIds.Add(request.RoleId);
                await _userRepository.UpdateAsync(userToAssign);

                // Cache'i temizle
                var cacheKey = $"permissions:{tenantId}:{request.UserId}";
                await _cacheService.RemoveAsync(cacheKey);
            }
        }
    }
} 