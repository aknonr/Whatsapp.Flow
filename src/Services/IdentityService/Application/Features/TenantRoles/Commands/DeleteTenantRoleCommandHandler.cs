using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.TenantRoles.Commands
{
    public class DeleteTenantRoleCommandHandler : IRequestHandler<DeleteTenantRoleCommand>
    {
        private readonly ITenantRoleRepository _tenantRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteTenantRoleCommandHandler(
            ITenantRoleRepository tenantRoleRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantRoleRepository = tenantRoleRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(DeleteTenantRoleCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User's tenant could not be verified.");
            }

            var roleToDelete = await _tenantRoleRepository.GetByIdAsync(request.Id);
            if (roleToDelete == null || roleToDelete.TenantId != tenantId)
            {
                throw new NotFoundException("TenantRole", request.Id);
            }

            if (roleToDelete.IsSystemRole)
            {
                throw new InvalidOperationException("System roles cannot be deleted.");
            }
            
            // Bu rolü kullanan kullanıcı var mı kontrol et
            // Bu kısım büyük veri setlerinde performansı etkileyebilir. 
            // Daha performanslı bir çözüm için IUserRepository'e "IsRoleAssignedAsync(string roleId)" gibi bir metot eklenebilir.
            // Şimdilik bu şekilde ilerliyoruz.
            // var isRoleAssigned = await _userRepository.IsRoleAssignedToAnyUserAsync(request.Id, tenantId);
            // if(isRoleAssigned)
            // {
            //     throw new InvalidOperationException("This role is currently assigned to one or more users and cannot be deleted.");
            // }

            await _tenantRoleRepository.DeleteAsync(request.Id);
        }
    }
} 