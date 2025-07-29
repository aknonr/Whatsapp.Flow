using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Entities;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(currentUserId))
            {
                throw new UnauthorizedAccessException("User identity could not be verified.");
            }

            if (request.Id == currentUserId)
            {
                throw new InvalidOperationException("You cannot delete your own account.");
            }

            var userToDelete = await _userRepository.GetByIdAsync(request.Id);

            if (userToDelete == null || userToDelete.TenantId != tenantId)
            {
                throw new NotFoundException("User", request.Id);
            }

            userToDelete.Status = UserStatus.Inactive; 

            await _userRepository.UpdateAsync(userToDelete);
        }
    }
} 