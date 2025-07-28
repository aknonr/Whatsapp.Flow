using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Whatsapp.Flow.Services.Identity.API.Security;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Commands;
using Whatsapp.Flow.Services.Identity.Application.Features.Tenant.Queries;
using Whatsapp.Flow.Services.Identity.Domain.Entities;

namespace Whatsapp.Flow.Services.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantCommand command)
        {
            var tenantId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTenantById), new { id = tenantId }, null);
        }

        [HttpGet("{id}")]
        [HasRole(Role.SuperAdmin)]
        public async Task<IActionResult> GetTenantById(string id)
        {
            var query = new GetTenantByIdQuery(id);
            var tenant = await _mediator.Send(query);
            return Ok(tenant);
        }
    }
} 