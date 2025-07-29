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
    public class UpdateTenantRoleCommandHandler : IRequestHandler<UpdateTenantRoleCommand>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateTenantRoleCommandHandler(
            ITenantRoleRepository tenantRoleRepository, 
            IUserRepository userRepository,
            ICacheService cacheService,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _userRepository = userRepository;
            _cacheService = cacheService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(UpdateTenantRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User's tenant could not be verified.");
            }

            var roleToUpdate = await _tenantRoleRepository.GetByIdAsync(request.Id);

            if (roleToUpdate == null || roleToUpdate.TenantId != tenantId)
            {
                throw new NotFoundException("TenantRole", request.Id);
            }

            if (roleToUpdate.IsSystemRole)
            {
                throw new InvalidOperationException("System roles cannot be modified.");
            }

            // Rolü güncellemeden önce bu role sahip kullanıcıları bul
            var usersWithThisRole = await _userRepository.GetUsersByRoleIdAsync(request.Id, tenantId);

            roleToUpdate.Description = request.Description;
            roleToUpdate.Permissions = request.Permissions ?? new();

            await _tenantRoleRepository.UpdateAsync(roleToUpdate);

            // Güncellemeden sonra, etkilenen tüm kullanıcıların cache'ini temizle
            foreach (var user in usersWithThisRole)
            {
                var cacheKey = $"permissions:{tenantId}:{user.Id}";
                await _cacheService.RemoveAsync(cacheKey);
            }
        }
    }
} 