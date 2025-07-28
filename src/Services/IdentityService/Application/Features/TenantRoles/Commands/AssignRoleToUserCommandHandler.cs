using MediatR;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssignRoleToUserCommandHandler(
            IUserRepository userRepository,
            ITenantRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var requestingTenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(requestingTenantId))
            {
                throw new UnauthorizedAccessException("Requesting user's tenant could not be verified.");
            }

            var userToUpdate = await _userRepository.GetByIdAsync(request.UserId);
            if (userToUpdate == null || userToUpdate.TenantId != requestingTenantId)
            {
                throw new InvalidOperationException("Target user not found or is not in the same tenant.");
            }

            var roleToAssign = await _roleRepository.GetByIdAsync(request.RoleId);
            if (roleToAssign == null || roleToAssign.TenantId != requestingTenantId)
            {
                throw new InvalidOperationException("Role not found or is not in the same tenant.");
            }

            if (userToUpdate.TenantRoleIds.Contains(request.RoleId))
            {
                // Kullanıcının zaten bu rolü var, bir şey yapmaya gerek yok.
                // İsteğe bağlı olarak bir bildirim dönebilir veya sadece başarılı kabul edebiliriz.
                return;
            }

            userToUpdate.TenantRoleIds.Add(request.RoleId);
            await _userRepository.UpdateAsync(userToUpdate);
        }
    }
} 