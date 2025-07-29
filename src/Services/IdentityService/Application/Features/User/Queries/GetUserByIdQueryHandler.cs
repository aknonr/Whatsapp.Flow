using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _httpContextAccessor.HttpContext.User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                throw new UnauthorizedAccessException("User's tenant could not be verified.");
            }

            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null || user.TenantId != tenantId)
            {
                throw new NotFoundException("User", request.Id);
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Status = user.Status,
                SystemRole = user.SystemRole,
                TenantRoleIds = user.TenantRoleIds,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
} 