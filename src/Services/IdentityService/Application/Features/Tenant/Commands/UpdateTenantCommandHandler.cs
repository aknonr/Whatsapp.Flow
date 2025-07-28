using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands
{
    public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand>
    {
        private readonly ITenantRepository _tenantRepository;

        public UpdateTenantCommandHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantToUpdate = await _tenantRepository.GetByIdAsync(request.Id);

            if (tenantToUpdate == null)
            {
                throw new NotFoundException(nameof(tenantToUpdate), request.Id);
            }

            // Gelen verilerle entity'yi güncelle
            tenantToUpdate.Name = request.Name;
            tenantToUpdate.CompanyName = request.CompanyName;
            tenantToUpdate.ContactEmail = request.ContactEmail;
            tenantToUpdate.ContactPhone = request.ContactPhone;

            await _tenantRepository.UpdateAsync(tenantToUpdate);
        }
    }
} 