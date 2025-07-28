using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand>
    {
        private readonly ITenantRepository _tenantRepository;

        public DeleteTenantCommandHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantToDelete = await _tenantRepository.GetByIdAsync(request.Id);

            if (tenantToDelete == null)
            {
                throw new NotFoundException(nameof(tenantToDelete), request.Id);
            }

            // Soft delete işlemi
            await _tenantRepository.SoftDeleteAsync(request.Id);
        }
    }
} 