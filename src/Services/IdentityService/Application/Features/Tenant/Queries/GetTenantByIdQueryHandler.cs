using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.Application.Exceptions;
using Whatsapp.Flow.Services.Identity.Domain.Repositories;

namespace Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries
{
    public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDto>
    {
        private readonly ITenantRepository _tenantRepository;

        public GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByIdAsync(request.Id);

            if (tenant == null)
            {
                throw new NotFoundException(nameof(tenant), request.Id);
            }

            return new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                CompanyName = tenant.CompanyName,
                ContactEmail = tenant.ContactEmail,
                ContactPhone = tenant.ContactPhone,
                Status = tenant.Status.ToString(),
                CreatedAt = tenant.CreatedAt,
                ActivatedAt = tenant.ActivatedAt
            };
        }
    }
} 