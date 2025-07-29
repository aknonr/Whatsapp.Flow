using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateUserCommandHandler(IUserRepository userRepository, ITenantRoleRepository tenantRoleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _tenantRoleRepository = tenantRoleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User's tenant could not be verified.");
            }

            var userToUpdate = await _userRepository.GetByIdAsync(request.Id);

            if (userToUpdate == null || userToUpdate.TenantId != tenantId)
            {
                throw new NotFoundException("User", request.Id);
            }
            
            // Atanmak istenen rollerin bu tenant'a ait olup olmadığını kontrol et
            if (request.TenantRoleIds != null && request.TenantRoleIds.Any())
            {
                var tenantRoles = await _tenantRoleRepository.GetByTenantIdAsync(tenantId);
                var tenantRoleIds = tenantRoles.Select(r => r.Id).ToList();

                var invalidRoles = request.TenantRoleIds.Except(tenantRoleIds).ToList();
                if (invalidRoles.Any())
                {
                    throw new InvalidOperationException($"Roles are not valid for this tenant: {string.Join(", ", invalidRoles)}");
                }

                userToUpdate.TenantRoleIds = request.TenantRoleIds;
            }


            userToUpdate.FirstName = request.FirstName;
            userToUpdate.LastName = request.LastName;
            userToUpdate.Status = request.Status;
            
            await _userRepository.UpdateAsync(userToUpdate);
        }
    }
} 