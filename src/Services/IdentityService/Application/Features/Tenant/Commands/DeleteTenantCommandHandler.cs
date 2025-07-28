using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteTenantCommandHandler(ITenantRepository tenantRepository, IHttpContextAccessor httpContextAccessor)
        {
            _tenantRepository = tenantRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantToDelete = await _tenantRepository.GetByIdAsync(request.Id);
            if (tenantToDelete == null)
            {
                throw new NotFoundException("Tenant", request.Id);
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // Normalde bu durum [Authorize] attribute'u nedeniyle oluşmaz, ama bir güvence katmanıdır.
                throw new System.UnauthorizedAccessException("User ID could not be determined for auditing.");
            }

            await _tenantRepository.SoftDeleteAsync(request.Id, userId);
        }
    }
} 
} 